/**
 * Autogenerated by Thrift Compiler (0.9.0)
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

namespace de.dfki.tecs.robot.baxter
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class RetrieveAnglesEvent : TBase
  {
    private Angles _angles;

    public Angles Angles
    {
      get
      {
        return _angles;
      }
      set
      {
        __isset.angles = true;
        this._angles = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool angles;
    }

    public RetrieveAnglesEvent() {
      this._angles = new Angles();
    }

    public void Read (TProtocol iprot)
    {
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
            if (field.Type == TType.Struct) {
              Angles = new Angles();
              Angles.Read(iprot);
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
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("RetrieveAnglesEvent");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Angles != null && __isset.angles) {
        field.Name = "angles";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        Angles.Write(oprot);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("RetrieveAnglesEvent(");
      sb.Append("Angles: ");
      sb.Append(Angles== null ? "<null>" : Angles.ToString());
      sb.Append(")");
      return sb.ToString();
    }

  }

}
