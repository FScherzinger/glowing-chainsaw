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
  public partial class InformationEvent : TBase
  {
    private string _informtion;

    /// <summary>
    /// 
    /// <seealso cref="Device"/>
    /// </summary>
    public Device Type { get; set; }

    public Position Inspect_pos { get; set; }

    public int Id { get; set; }

    public string Informtion
    {
      get
      {
        return _informtion;
      }
      set
      {
        __isset.informtion = true;
        this._informtion = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool informtion;
    }

    public InformationEvent() {
    }

    public InformationEvent(Device type, Position inspect_pos, int Id) : this() {
      this.Type = type;
      this.Inspect_pos = inspect_pos;
      this.Id = Id;
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        bool isset_type = false;
        bool isset_inspect_pos = false;
        bool isset_Id = false;
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
              if (field.Type == TType.Struct) {
                Inspect_pos = new Position();
                Inspect_pos.Read(iprot);
                isset_inspect_pos = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.I32) {
                Id = iprot.ReadI32();
                isset_Id = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.String) {
                Informtion = iprot.ReadString();
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
        if (!isset_inspect_pos)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_Id)
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
        TStruct struc = new TStruct("InformationEvent");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        field.Name = "type";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32((int)Type);
        oprot.WriteFieldEnd();
        field.Name = "inspect_pos";
        field.Type = TType.Struct;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        Inspect_pos.Write(oprot);
        oprot.WriteFieldEnd();
        field.Name = "Id";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Id);
        oprot.WriteFieldEnd();
        if (Informtion != null && __isset.informtion) {
          field.Name = "informtion";
          field.Type = TType.String;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Informtion);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("InformationEvent(");
      __sb.Append(", Type: ");
      __sb.Append(Type);
      __sb.Append(", Inspect_pos: ");
      __sb.Append(Inspect_pos== null ? "<null>" : Inspect_pos.ToString());
      __sb.Append(", Id: ");
      __sb.Append(Id);
      if (Informtion != null && __isset.informtion) {
        __sb.Append(", Informtion: ");
        __sb.Append(Informtion);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
