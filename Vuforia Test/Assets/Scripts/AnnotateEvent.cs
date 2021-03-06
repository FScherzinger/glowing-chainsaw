/**
 * Autogenerated by Thrift Compiler (0.9.3)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace de.dfki.events
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class AnnotateEvent : TBase
  {

    /// <summary>
    /// 
    /// <seealso cref="Device"/>
    /// </summary>
    public Device Type { get; set; }

    public int ObjectID { get; set; }

    public int Id { get; set; }

    public string Information { get; set; }

    public AnnotateEvent() {
    }

    public AnnotateEvent(Device type, int ObjectID, int Id, string information) : this() {
      this.Type = type;
      this.ObjectID = ObjectID;
      this.Id = Id;
      this.Information = information;
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        bool isset_type = false;
        bool isset_ObjectID = false;
        bool isset_Id = false;
        bool isset_information = false;
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.I32) {
                Type = (Device)iprot.ReadI32();
                isset_type = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.I32) {
                ObjectID = iprot.ReadI32();
                isset_ObjectID = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.I32) {
                Id = iprot.ReadI32();
                isset_Id = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.String) {
                Information = iprot.ReadString();
                isset_information = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
        if (!isset_type)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_ObjectID)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_Id)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_information)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public void Write(TProtocol oprot) {
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct("AnnotateEvent");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        field.Name = "type";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32((int)Type);
        oprot.WriteFieldEnd();
        field.Name = "ObjectID";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ObjectID);
        oprot.WriteFieldEnd();
        field.Name = "information";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Information);
        oprot.WriteFieldEnd();
        field.Name = "Id";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Id);
        oprot.WriteFieldEnd();
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("AnnotateEvent(");
      __sb.Append(", Type: ");
      __sb.Append(Type);
      __sb.Append(", ObjectID: ");
      __sb.Append(ObjectID);
      __sb.Append(", Id: ");
      __sb.Append(Id);
      __sb.Append(", Information: ");
      __sb.Append(Information);
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
