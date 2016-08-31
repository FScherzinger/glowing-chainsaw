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
  public partial class Position : TBase
  {
    private string _X_left;
    private string _Y_left;
    private string _Z_left;
    private string _X_right;
    private string _Y_right;
    private string _Z_right;

    public string X_left
    {
      get
      {
        return _X_left;
      }
      set
      {
        __isset.X_left = true;
        this._X_left = value;
      }
    }

    public string Y_left
    {
      get
      {
        return _Y_left;
      }
      set
      {
        __isset.Y_left = true;
        this._Y_left = value;
      }
    }

    public string Z_left
    {
      get
      {
        return _Z_left;
      }
      set
      {
        __isset.Z_left = true;
        this._Z_left = value;
      }
    }

    public string X_right
    {
      get
      {
        return _X_right;
      }
      set
      {
        __isset.X_right = true;
        this._X_right = value;
      }
    }

    public string Y_right
    {
      get
      {
        return _Y_right;
      }
      set
      {
        __isset.Y_right = true;
        this._Y_right = value;
      }
    }

    public string Z_right
    {
      get
      {
        return _Z_right;
      }
      set
      {
        __isset.Z_right = true;
        this._Z_right = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool X_left;
      public bool Y_left;
      public bool Z_left;
      public bool X_right;
      public bool Y_right;
      public bool Z_right;
    }

    public Position() {
      this._X_left = "";
      this._Y_left = "";
      this._Z_left = "";
      this._X_right = "";
      this._Y_right = "";
      this._Z_right = "";
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
            if (field.Type == TType.String) {
              X_left = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.String) {
              Y_left = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.String) {
              Z_left = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.String) {
              X_right = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.String) {
              Y_right = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.String) {
              Z_right = iprot.ReadString();
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
      TStruct struc = new TStruct("Position");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (X_left != null && __isset.X_left) {
        field.Name = "X_left";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(X_left);
        oprot.WriteFieldEnd();
      }
      if (Y_left != null && __isset.Y_left) {
        field.Name = "Y_left";
        field.Type = TType.String;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Y_left);
        oprot.WriteFieldEnd();
      }
      if (Z_left != null && __isset.Z_left) {
        field.Name = "Z_left";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Z_left);
        oprot.WriteFieldEnd();
      }
      if (X_right != null && __isset.X_right) {
        field.Name = "X_right";
        field.Type = TType.String;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(X_right);
        oprot.WriteFieldEnd();
      }
      if (Y_right != null && __isset.Y_right) {
        field.Name = "Y_right";
        field.Type = TType.String;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Y_right);
        oprot.WriteFieldEnd();
      }
      if (Z_right != null && __isset.Z_right) {
        field.Name = "Z_right";
        field.Type = TType.String;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Z_right);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("Position(");
      sb.Append("X_left: ");
      sb.Append(X_left);
      sb.Append(",Y_left: ");
      sb.Append(Y_left);
      sb.Append(",Z_left: ");
      sb.Append(Z_left);
      sb.Append(",X_right: ");
      sb.Append(X_right);
      sb.Append(",Y_right: ");
      sb.Append(Y_right);
      sb.Append(",Z_right: ");
      sb.Append(Z_right);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
