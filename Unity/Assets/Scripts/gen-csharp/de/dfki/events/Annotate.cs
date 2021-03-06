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
  public partial class Annotate : TBase
  {
    private Position _position;
    private int _objectId;
    private string _information;

    public int Id { get; set; }

    /// <summary>
    /// 
    /// <seealso cref="Device"/>
    /// </summary>
    public Device Type { get; set; }

    public Position Position
    {
      get
      {
        return _position;
      }
      set
      {
        __isset.position = true;
        this._position = value;
      }
    }

    public int ObjectId
    {
      get
      {
        return _objectId;
      }
      set
      {
        __isset.objectId = true;
        this._objectId = value;
      }
    }

    public string Information
    {
      get
      {
        return _information;
      }
      set
      {
        __isset.information = true;
        this._information = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool position;
      public bool objectId;
      public bool information;
    }

    public Annotate() {
    }

    public Annotate(int Id, Device type) : this() {
      this.Id = Id;
      this.Type = type;
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        bool isset_Id = false;
        bool isset_type = false;
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
                Id = iprot.ReadI32();
                isset_Id = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.I32) {
                Type = (Device)iprot.ReadI32();
                isset_type = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.Struct) {
                Position = new Position();
                Position.Read(iprot);
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.I32) {
                ObjectId = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 5:
              if (field.Type == TType.String) {
                Information = iprot.ReadString();
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
        if (!isset_Id)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_type)
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
        TStruct struc = new TStruct("Annotate");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        field.Name = "Id";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Id);
        oprot.WriteFieldEnd();
        field.Name = "type";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32((int)Type);
        oprot.WriteFieldEnd();
        if (Position != null && __isset.position) {
          field.Name = "position";
          field.Type = TType.Struct;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          Position.Write(oprot);
          oprot.WriteFieldEnd();
        }
        if (__isset.objectId) {
          field.Name = "objectId";
          field.Type = TType.I32;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(ObjectId);
          oprot.WriteFieldEnd();
        }
        if (Information != null && __isset.information) {
          field.Name = "information";
          field.Type = TType.String;
          field.ID = 5;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Information);
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
      StringBuilder __sb = new StringBuilder("Annotate(");
      __sb.Append(", Id: ");
      __sb.Append(Id);
      __sb.Append(", Type: ");
      __sb.Append(Type);
      if (Position != null && __isset.position) {
        __sb.Append(", Position: ");
        __sb.Append(Position== null ? "<null>" : Position.ToString());
      }
      if (__isset.objectId) {
        __sb.Append(", ObjectId: ");
        __sb.Append(ObjectId);
      }
      if (Information != null && __isset.information) {
        __sb.Append(", Information: ");
        __sb.Append(Information);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
