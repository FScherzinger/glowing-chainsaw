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
  public partial class Direction : TBase
  {

    /// <summary>
    /// X-Coordinate *
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Y-Coordinate *
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Z-Coordinate *
    /// </summary>
    public double Z { get; set; }

    /// <summary>
    /// W-Coordinate *
    /// </summary>
    public double W { get; set; }

    public Direction() {
    }

    public Direction(double x, double y, double z, double w) : this() {
      this.X = x;
      this.Y = y;
      this.Z = z;
      this.W = w;
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        bool isset_x = false;
        bool isset_y = false;
        bool isset_z = false;
        bool isset_w = false;
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
              if (field.Type == TType.Double) {
                X = iprot.ReadDouble();
                isset_x = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.Double) {
                Y = iprot.ReadDouble();
                isset_y = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.Double) {
                Z = iprot.ReadDouble();
                isset_z = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.Double) {
                W = iprot.ReadDouble();
                isset_w = true;
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
        if (!isset_x)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_y)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_z)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_w)
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
        TStruct struc = new TStruct("Direction");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        field.Name = "x";
        field.Type = TType.Double;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteDouble(X);
        oprot.WriteFieldEnd();
        field.Name = "y";
        field.Type = TType.Double;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteDouble(Y);
        oprot.WriteFieldEnd();
        field.Name = "z";
        field.Type = TType.Double;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteDouble(Z);
        oprot.WriteFieldEnd();
        field.Name = "w";
        field.Type = TType.Double;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteDouble(W);
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
      StringBuilder __sb = new StringBuilder("Direction(");
      __sb.Append(", X: ");
      __sb.Append(X);
      __sb.Append(", Y: ");
      __sb.Append(Y);
      __sb.Append(", Z: ");
      __sb.Append(Z);
      __sb.Append(", W: ");
      __sb.Append(W);
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
