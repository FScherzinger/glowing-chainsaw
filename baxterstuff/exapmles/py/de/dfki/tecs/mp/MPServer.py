#!/usr/bin/python2.7
# -*- coding: utf-8 -*-

# The Creative Commons CC-BY-NC 4.0 License
#
# http://creativecommons.org/licenses/by-nc/4.0/legalcode
#
# Creative Commons (CC) by DFKI GmbH
#  - Christian Bürckert <Christian.Buerckert@DFKI.de>
#  - Yannick Körber <Yannick.Koerber@DFKI.de>
#  - Magdalena Kaiser <Magdalena.Kaiser@DFKI.de>

# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
# FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
# IN THE SOFTWARE.
#

import sys, glob
import socket
import time
import os
import platform
import threading

from thrift import Thrift

from de.dfki.tecs.misc.IntervalHelper import IntervalHelper
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.Event import Event
from de.dfki.tecs.mp.MPServerSocket import MPServerSocket
from de.dfki.tecs.misc.Serializer import Serializer



	# TCPServer for message-based communication. Handles multiple tcp connections
	# in one thread (+ one thread for accepting new connections). See MPServer::send for sending messages to clients.
	# See MPDistributeServer::recv for receiving messages from clients.
	# This implementation provides an easy way to set up a server with multiple clients.
	# Not thread-safe
	# <p/>
	# <p/>
	# Example: (in java)
	# MPServer server = new MPServer(new MPServerSocket(5000, MPServer.createService("mp-ping-pong")));
	# server.start();
	# ...
	# server.send("test-event", new byte[1024]); //sends a new message to all connected clients
	# Event event = server.recv();
	# if(event != null){
	# //got a new message from event.getSource();
	# server.send(event.getSource(), "test-event", new byte[1024]); //respond to this client



class MPServer:


	# Creates a new server to communicate via TECS-MP.
	def __init__(self, serverSocket):
		self.__serverSocket = serverSocket
		self.__callback = None
		self.__connections = dict()
		self.__events = []
		self.__acceptIH = IntervalHelper(50)
		self.__serializer = Serializer()
		self.__flag = 1


	def getCallback(self):
		return self.__callback


	 # @param callback Sets the callback. It informs about the server's state. None if you want to unset it.
	 # @throws SocketError If closing the socket fails. See MPServerSocket::close
	def setCallback(callback):
		self.__callback = callback


	 # @return true iff a new client was accepted
	 # @throws SocketError If the accept operation fails. 
	 # @throws ValueError If the server socket is not bound.
	def updateAccept(self):
		if not self.__acceptIH.shouldExecute():
			return False
		#check if there is a new client to accept
		socket = self.__serverSocket.accept()
		if socket is None:
			#no new client
			return False
		id = socket.getRemoteHost() + ":" + str(socket.getRemotePort())
		print "New client accepted: " + str(id)
		self.__connections[str(id)] = socket
		if self.__callback:
			self.__callback.connectionEstablished(self, str(id))
		return True


	 # Binds the underlying server socket such that clients can connect.
	 # All queued events are discarded.
	 # @throws ValueError if the socket is already bound
	 # @throws SocketError if binding fails
	def bind(self):
		self.__serverSocket.bind()
		self.__events = []
		try:
			#set to non-blocking mode
			self.__serverSocket.setBlocking(False)
		except Exception as e:
			try:
				if self.__serverSocket.isBound():
					self.__serverSocket.unbind()
			except Exception as e2:
				print "Set non-blocking mode failed. And unbinding server socket failed too: " + str(e2)
			raise e
		print "Bound to port " + str(self.__serverSocket.getPort())
		if self.__callback:
			self.__callback.serverInitialized(self)
			services = self.__serverSocket.getOfferedServices()
			if services:
				self.__callback.discoveryInitialized(self, services)
 


	 # @throws SocketError: If closing the server socket fails. See MPServerSocket::close
	 # @throws ValueError If the socket is not bound.
	def unbind(self):
		clientIds = self.getClientIds()
		for clientId in clientIds:
			self.disconnect(clientId, "Unbind called")
		if self.__serverSocket.isBound():
			self.__serverSocket.unbind()
		print "Socket unbound"
		if self.__callback:
			self.__callback.serverDeinitialzed(self)
	 

	 # Updates the internal state of the server. New clients are accepted and messages are received.
	 # @throws ValueError If the socket is not bound.
	def _update(self):
		if not self.__serverSocket.isBound():
			raise ValueError("Can't update: Not bound")
		workDone = False   
		workDone = workDone | self.updateAccept()
		workDone = workDone | self._updateConnections()
		return workDone


	# Reads and queue messages from all connected clients.
	# @return If at least one message was received
	def _updateConnections(self):
		workDone = False
		lostConnections = dict()
		#go over all available connections
		for key in self.__connections:
			try:
				value = self.__connections[key]
				#receive event from current connection
				event = value.recv()
				if event:
					event.setSource(key) #use the source field of events to set the client id
					self.__events.append(event)
					workDone = True
				elif self.__connections[key].isReceivingData():
					workDone = True;
			except Exception as e:       #SocketAccessException ex) {
				#print "Lost connection to " + key + " Cause: " + ex)
				lostConnections[key] =  e
	    #announce the information later to callback => calling disconnect is safe from callback
		for key in lostConnections:
	 		self.disconnect(key, lostConnections[key])
		return workDone


	# Disconnect the client with the given id.
	def disconnect(self, clientId, cause=None):
		if cause is None:
			cause = "Your application called disconnect"
		if not clientId in self.__connections:
			return False
		clientSocket = self.__connections[clientId]
		if clientSocket.isConnected():
			try:
				clientSocket.disconnect()
			except Exception as e: #(SocketAccessException ex) {
				print "Could not close client socket " + str(clientId) + ". Cause: " + str(e)
		self.__connections.pop(clientId, None)
	 	print "Disconnected client " + str(clientId) + ". Cause: " + str(cause)
		if self.__callback:
			self.__callback.connectionLost(self, clientId);
		return True


	# @return True iff this server is bound and ready for communication
	def isBound(self):
	 	return self.__serverSocket.isBound()


	 # This method returns all events that were received from all clients since the last call of recv.
	 # Each event has string identifier for its source (see Event::getHeader()::getSource()). You can this identifier
	 # to send an event to this connection.
	 # @param outEvents All received events are transferred to this collection.
	 # @return the given argument outEvent
	def recvAll(self, outEvents=None):
		self._update()
		if outEvents is None:
			events_ = self.__events
			self.__events =[]
			return events_
		else:
			if self.__events == []:
				return
			outEvents = self.__events
			self.__events = []
			return




	# Use Event::getSource to identify the sender.
	# no arguments: return None if no event is available. Otherwise it pops the oldest queued event (all clients.)
	# @param outEvents events are stored in this collection 
	# @param clientId  The id of the client you want to receive an event from
	# if both parameter are given: Pops all queued events for the client with the given id.
	# if only clientId is present: The oldest event from the client with the given id if available. Otherwise None.
	def recv(self, outEvents=None, clientId=None):
		self._update()
		if outEvents is None and clientId is None:
			if len(self.__events) > 0:
				#return oldest received event
				firstEvent = self.__events.pop(0)
				return firstEvent
			else:
				return None
		newEventList = self.__events
		for event in self.__events:
			if event.getSource() == clientId:
				if outEvents:
					#write all events with given id into collection
					outEvents.append(event)
				else:
					#return oldest event with given id
					return event	
			else:
				newEventList.remove(event)
		self.__events = newEventList
		return None



	 # Sends the given message to all clients.
	 # @return If there is at least one connected client.
	 # @throws RuntimeException      if the message can't be serialized
	 # @throws ValueError if the server is not bound.
	def send(self, target=None, etype=None, msg=None):
		if etype is None or msg is None:
			return False

		if target is None:
			if not self.__serverSocket.isBound():
				raise ValueError("Can't send message: MPServer is not bound.")
			self.updateAccept()
			#send to all clients
			for targetId in self.getClientIds():
				self.send(targetId, etype, msg)
			return True	
		else:
			#Sends the given event to a specific client. The function returns prematurely if no client
			#with the given id is known (and prints a warning).
			if not self.__serverSocket.isBound():
				raise ValueError("Can't send message: MPServer is not bound.")
			self.updateAccept();
			for connection in self.__connections:
				if connection == target:
					socket = self.__connections[target]
			if socket is None or not socket.isConnected():
				print "Can't send message %s to %s: No connection available", str(etype), str(target)
				return False
			try:
				#send message
				socket.send(etype, msg);
				return True;
			except Exception as ex:
				self.disconnect(target, "Sending failed: " + str(ex))
				return False


	# @return The port of the server socket
	def getPort(self):
		return self.__serverSocket.getPort()


	# @return A list containing the ids of all connections. You can use those ids to send events to specific clients.
	def getClientIds(self):
		return list(self.__connections.keys())


	def toString(self):
		sb = "MPServer { "
		sb += "serverSocket=" + str(self.__serverSocket)
		sb += ", connections=["
		for connection in self.__connections.keys:
		    sb += str(connection) + (" connected:") + str(self.__connections[connection].isConnected()) + ", "
		sb += "]"
		sb += " }"
		return sb


	# @deprecated Use MPServerSocket.createService
	@staticmethod
	def createService(serviceType):
		return MPServerSocket.createService(serviceType)




class Callback:


	# Called once after server could intialize successfully. That means that the server is ready to operate
	def serverInitialized(self, server):
		pass


	# Called
	# @param server
	def serverDeinitialzed(self, server):
		pass

	 # Called once as soon as client connected.
	 # @param server
	 # @param clientId
	def connectionEstablished(self, server, clientId):
		pass


	 # Called once as soon as the connection to a client was lost.
	 # @param server
	 # @param clientId
	def connectionLost(self, server, clientId):
		pass


	 # Called after the discovery was started.
	 # @param server
	 # @param services The offered services. (copy)
	def discoveryInitialized(self, server, services):
		pass

