/**
 * Autogenerated by Thrift Compiler (0.9.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
package de.dfki.tecs.robot.baxter;

import org.apache.thrift.scheme.IScheme;
import org.apache.thrift.scheme.SchemeFactory;
import org.apache.thrift.scheme.StandardScheme;

import org.apache.thrift.scheme.TupleScheme;
import org.apache.thrift.protocol.TTupleProtocol;
import org.apache.thrift.protocol.TProtocolException;
import org.apache.thrift.EncodingUtils;
import org.apache.thrift.TException;
import java.util.List;
import java.util.ArrayList;
import java.util.Map;
import java.util.HashMap;
import java.util.EnumMap;
import java.util.Set;
import java.util.HashSet;
import java.util.EnumSet;
import java.util.Collections;
import java.util.BitSet;
import java.nio.ByteBuffer;
import java.util.Arrays;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class PickAndPlaceEvent implements org.apache.thrift.TBase<PickAndPlaceEvent, PickAndPlaceEvent._Fields>, java.io.Serializable, Cloneable {
  private static final org.apache.thrift.protocol.TStruct STRUCT_DESC = new org.apache.thrift.protocol.TStruct("PickAndPlaceEvent");

  private static final org.apache.thrift.protocol.TField LIMB_FIELD_DESC = new org.apache.thrift.protocol.TField("limb", org.apache.thrift.protocol.TType.I32, (short)1);
  private static final org.apache.thrift.protocol.TField INITIAL_POS_FIELD_DESC = new org.apache.thrift.protocol.TField("initial_pos", org.apache.thrift.protocol.TType.STRUCT, (short)2);
  private static final org.apache.thrift.protocol.TField FINAL_POS_FIELD_DESC = new org.apache.thrift.protocol.TField("final_pos", org.apache.thrift.protocol.TType.STRUCT, (short)3);
  private static final org.apache.thrift.protocol.TField INITIAL_ORI_FIELD_DESC = new org.apache.thrift.protocol.TField("initial_ori", org.apache.thrift.protocol.TType.STRUCT, (short)4);
  private static final org.apache.thrift.protocol.TField FINAL_ORI_FIELD_DESC = new org.apache.thrift.protocol.TField("final_ori", org.apache.thrift.protocol.TType.STRUCT, (short)5);
  private static final org.apache.thrift.protocol.TField SPEED_FIELD_DESC = new org.apache.thrift.protocol.TField("speed", org.apache.thrift.protocol.TType.STRUCT, (short)6);
  private static final org.apache.thrift.protocol.TField ANGLS_FIELD_DESC = new org.apache.thrift.protocol.TField("angls", org.apache.thrift.protocol.TType.STRUCT, (short)7);
  private static final org.apache.thrift.protocol.TField MODE_FIELD_DESC = new org.apache.thrift.protocol.TField("mode", org.apache.thrift.protocol.TType.I32, (short)8);
  private static final org.apache.thrift.protocol.TField KIN_FIELD_DESC = new org.apache.thrift.protocol.TField("kin", org.apache.thrift.protocol.TType.I32, (short)9);

  private static final Map<Class<? extends IScheme>, SchemeFactory> schemes = new HashMap<Class<? extends IScheme>, SchemeFactory>();
  static {
    schemes.put(StandardScheme.class, new PickAndPlaceEventStandardSchemeFactory());
    schemes.put(TupleScheme.class, new PickAndPlaceEventTupleSchemeFactory());
  }

  /**
   * 
   * @see Limb
   */
  public Limb limb; // required
  public Position initial_pos; // required
  public Position final_pos; // required
  public Orientation initial_ori; // required
  public Orientation final_ori; // required
  public Speed speed; // required
  public Angles angls; // required
  /**
   * 
   * @see Reference_sys
   */
  public Reference_sys mode; // required
  /**
   * 
   * @see Kinematics
   */
  public Kinematics kin; // required

  /** The set of fields this struct contains, along with convenience methods for finding and manipulating them. */
  public enum _Fields implements org.apache.thrift.TFieldIdEnum {
    /**
     * 
     * @see Limb
     */
    LIMB((short)1, "limb"),
    INITIAL_POS((short)2, "initial_pos"),
    FINAL_POS((short)3, "final_pos"),
    INITIAL_ORI((short)4, "initial_ori"),
    FINAL_ORI((short)5, "final_ori"),
    SPEED((short)6, "speed"),
    ANGLS((short)7, "angls"),
    /**
     * 
     * @see Reference_sys
     */
    MODE((short)8, "mode"),
    /**
     * 
     * @see Kinematics
     */
    KIN((short)9, "kin");

    private static final Map<String, _Fields> byName = new HashMap<String, _Fields>();

    static {
      for (_Fields field : EnumSet.allOf(_Fields.class)) {
        byName.put(field.getFieldName(), field);
      }
    }

    /**
     * Find the _Fields constant that matches fieldId, or null if its not found.
     */
    public static _Fields findByThriftId(int fieldId) {
      switch(fieldId) {
        case 1: // LIMB
          return LIMB;
        case 2: // INITIAL_POS
          return INITIAL_POS;
        case 3: // FINAL_POS
          return FINAL_POS;
        case 4: // INITIAL_ORI
          return INITIAL_ORI;
        case 5: // FINAL_ORI
          return FINAL_ORI;
        case 6: // SPEED
          return SPEED;
        case 7: // ANGLS
          return ANGLS;
        case 8: // MODE
          return MODE;
        case 9: // KIN
          return KIN;
        default:
          return null;
      }
    }

    /**
     * Find the _Fields constant that matches fieldId, throwing an exception
     * if it is not found.
     */
    public static _Fields findByThriftIdOrThrow(int fieldId) {
      _Fields fields = findByThriftId(fieldId);
      if (fields == null) throw new IllegalArgumentException("Field " + fieldId + " doesn't exist!");
      return fields;
    }

    /**
     * Find the _Fields constant that matches name, or null if its not found.
     */
    public static _Fields findByName(String name) {
      return byName.get(name);
    }

    private final short _thriftId;
    private final String _fieldName;

    _Fields(short thriftId, String fieldName) {
      _thriftId = thriftId;
      _fieldName = fieldName;
    }

    public short getThriftFieldId() {
      return _thriftId;
    }

    public String getFieldName() {
      return _fieldName;
    }
  }

  // isset id assignments
  public static final Map<_Fields, org.apache.thrift.meta_data.FieldMetaData> metaDataMap;
  static {
    Map<_Fields, org.apache.thrift.meta_data.FieldMetaData> tmpMap = new EnumMap<_Fields, org.apache.thrift.meta_data.FieldMetaData>(_Fields.class);
    tmpMap.put(_Fields.LIMB, new org.apache.thrift.meta_data.FieldMetaData("limb", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.EnumMetaData(org.apache.thrift.protocol.TType.ENUM, Limb.class)));
    tmpMap.put(_Fields.INITIAL_POS, new org.apache.thrift.meta_data.FieldMetaData("initial_pos", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.StructMetaData(org.apache.thrift.protocol.TType.STRUCT, Position.class)));
    tmpMap.put(_Fields.FINAL_POS, new org.apache.thrift.meta_data.FieldMetaData("final_pos", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.StructMetaData(org.apache.thrift.protocol.TType.STRUCT, Position.class)));
    tmpMap.put(_Fields.INITIAL_ORI, new org.apache.thrift.meta_data.FieldMetaData("initial_ori", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.StructMetaData(org.apache.thrift.protocol.TType.STRUCT, Orientation.class)));
    tmpMap.put(_Fields.FINAL_ORI, new org.apache.thrift.meta_data.FieldMetaData("final_ori", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.StructMetaData(org.apache.thrift.protocol.TType.STRUCT, Orientation.class)));
    tmpMap.put(_Fields.SPEED, new org.apache.thrift.meta_data.FieldMetaData("speed", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.StructMetaData(org.apache.thrift.protocol.TType.STRUCT, Speed.class)));
    tmpMap.put(_Fields.ANGLS, new org.apache.thrift.meta_data.FieldMetaData("angls", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.StructMetaData(org.apache.thrift.protocol.TType.STRUCT, Angles.class)));
    tmpMap.put(_Fields.MODE, new org.apache.thrift.meta_data.FieldMetaData("mode", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.EnumMetaData(org.apache.thrift.protocol.TType.ENUM, Reference_sys.class)));
    tmpMap.put(_Fields.KIN, new org.apache.thrift.meta_data.FieldMetaData("kin", org.apache.thrift.TFieldRequirementType.REQUIRED, 
        new org.apache.thrift.meta_data.EnumMetaData(org.apache.thrift.protocol.TType.ENUM, Kinematics.class)));
    metaDataMap = Collections.unmodifiableMap(tmpMap);
    org.apache.thrift.meta_data.FieldMetaData.addStructMetaDataMap(PickAndPlaceEvent.class, metaDataMap);
  }

  public PickAndPlaceEvent() {
    this.limb = de.dfki.tecs.robot.baxter.Limb.LEFT;

    this.initial_pos = new Position();

    this.final_pos = new Position();

    this.initial_ori = new Orientation();

    this.final_ori = new Orientation();

    this.speed = new Speed();

    this.angls = new Angles();

    this.mode = de.dfki.tecs.robot.baxter.Reference_sys.ABSOLUTE;

    this.kin = de.dfki.tecs.robot.baxter.Kinematics.INVERSE;

  }

  public PickAndPlaceEvent(
    Limb limb,
    Position initial_pos,
    Position final_pos,
    Orientation initial_ori,
    Orientation final_ori,
    Speed speed,
    Angles angls,
    Reference_sys mode,
    Kinematics kin)
  {
    this();
    this.limb = limb;
    this.initial_pos = initial_pos;
    this.final_pos = final_pos;
    this.initial_ori = initial_ori;
    this.final_ori = final_ori;
    this.speed = speed;
    this.angls = angls;
    this.mode = mode;
    this.kin = kin;
  }

  /**
   * Performs a deep copy on <i>other</i>.
   */
  public PickAndPlaceEvent(PickAndPlaceEvent other) {
    if (other.isSetLimb()) {
      this.limb = other.limb;
    }
    if (other.isSetInitial_pos()) {
      this.initial_pos = new Position(other.initial_pos);
    }
    if (other.isSetFinal_pos()) {
      this.final_pos = new Position(other.final_pos);
    }
    if (other.isSetInitial_ori()) {
      this.initial_ori = new Orientation(other.initial_ori);
    }
    if (other.isSetFinal_ori()) {
      this.final_ori = new Orientation(other.final_ori);
    }
    if (other.isSetSpeed()) {
      this.speed = new Speed(other.speed);
    }
    if (other.isSetAngls()) {
      this.angls = new Angles(other.angls);
    }
    if (other.isSetMode()) {
      this.mode = other.mode;
    }
    if (other.isSetKin()) {
      this.kin = other.kin;
    }
  }

  public PickAndPlaceEvent deepCopy() {
    return new PickAndPlaceEvent(this);
  }

  @Override
  public void clear() {
    this.limb = de.dfki.tecs.robot.baxter.Limb.LEFT;

    this.initial_pos = new Position();

    this.final_pos = new Position();

    this.initial_ori = new Orientation();

    this.final_ori = new Orientation();

    this.speed = new Speed();

    this.angls = new Angles();

    this.mode = de.dfki.tecs.robot.baxter.Reference_sys.ABSOLUTE;

    this.kin = de.dfki.tecs.robot.baxter.Kinematics.INVERSE;

  }

  /**
   * 
   * @see Limb
   */
  public Limb getLimb() {
    return this.limb;
  }

  /**
   * 
   * @see Limb
   */
  public PickAndPlaceEvent setLimb(Limb limb) {
    this.limb = limb;
    return this;
  }

  public void unsetLimb() {
    this.limb = null;
  }

  /** Returns true if field limb is set (has been assigned a value) and false otherwise */
  public boolean isSetLimb() {
    return this.limb != null;
  }

  public void setLimbIsSet(boolean value) {
    if (!value) {
      this.limb = null;
    }
  }

  public Position getInitial_pos() {
    return this.initial_pos;
  }

  public PickAndPlaceEvent setInitial_pos(Position initial_pos) {
    this.initial_pos = initial_pos;
    return this;
  }

  public void unsetInitial_pos() {
    this.initial_pos = null;
  }

  /** Returns true if field initial_pos is set (has been assigned a value) and false otherwise */
  public boolean isSetInitial_pos() {
    return this.initial_pos != null;
  }

  public void setInitial_posIsSet(boolean value) {
    if (!value) {
      this.initial_pos = null;
    }
  }

  public Position getFinal_pos() {
    return this.final_pos;
  }

  public PickAndPlaceEvent setFinal_pos(Position final_pos) {
    this.final_pos = final_pos;
    return this;
  }

  public void unsetFinal_pos() {
    this.final_pos = null;
  }

  /** Returns true if field final_pos is set (has been assigned a value) and false otherwise */
  public boolean isSetFinal_pos() {
    return this.final_pos != null;
  }

  public void setFinal_posIsSet(boolean value) {
    if (!value) {
      this.final_pos = null;
    }
  }

  public Orientation getInitial_ori() {
    return this.initial_ori;
  }

  public PickAndPlaceEvent setInitial_ori(Orientation initial_ori) {
    this.initial_ori = initial_ori;
    return this;
  }

  public void unsetInitial_ori() {
    this.initial_ori = null;
  }

  /** Returns true if field initial_ori is set (has been assigned a value) and false otherwise */
  public boolean isSetInitial_ori() {
    return this.initial_ori != null;
  }

  public void setInitial_oriIsSet(boolean value) {
    if (!value) {
      this.initial_ori = null;
    }
  }

  public Orientation getFinal_ori() {
    return this.final_ori;
  }

  public PickAndPlaceEvent setFinal_ori(Orientation final_ori) {
    this.final_ori = final_ori;
    return this;
  }

  public void unsetFinal_ori() {
    this.final_ori = null;
  }

  /** Returns true if field final_ori is set (has been assigned a value) and false otherwise */
  public boolean isSetFinal_ori() {
    return this.final_ori != null;
  }

  public void setFinal_oriIsSet(boolean value) {
    if (!value) {
      this.final_ori = null;
    }
  }

  public Speed getSpeed() {
    return this.speed;
  }

  public PickAndPlaceEvent setSpeed(Speed speed) {
    this.speed = speed;
    return this;
  }

  public void unsetSpeed() {
    this.speed = null;
  }

  /** Returns true if field speed is set (has been assigned a value) and false otherwise */
  public boolean isSetSpeed() {
    return this.speed != null;
  }

  public void setSpeedIsSet(boolean value) {
    if (!value) {
      this.speed = null;
    }
  }

  public Angles getAngls() {
    return this.angls;
  }

  public PickAndPlaceEvent setAngls(Angles angls) {
    this.angls = angls;
    return this;
  }

  public void unsetAngls() {
    this.angls = null;
  }

  /** Returns true if field angls is set (has been assigned a value) and false otherwise */
  public boolean isSetAngls() {
    return this.angls != null;
  }

  public void setAnglsIsSet(boolean value) {
    if (!value) {
      this.angls = null;
    }
  }

  /**
   * 
   * @see Reference_sys
   */
  public Reference_sys getMode() {
    return this.mode;
  }

  /**
   * 
   * @see Reference_sys
   */
  public PickAndPlaceEvent setMode(Reference_sys mode) {
    this.mode = mode;
    return this;
  }

  public void unsetMode() {
    this.mode = null;
  }

  /** Returns true if field mode is set (has been assigned a value) and false otherwise */
  public boolean isSetMode() {
    return this.mode != null;
  }

  public void setModeIsSet(boolean value) {
    if (!value) {
      this.mode = null;
    }
  }

  /**
   * 
   * @see Kinematics
   */
  public Kinematics getKin() {
    return this.kin;
  }

  /**
   * 
   * @see Kinematics
   */
  public PickAndPlaceEvent setKin(Kinematics kin) {
    this.kin = kin;
    return this;
  }

  public void unsetKin() {
    this.kin = null;
  }

  /** Returns true if field kin is set (has been assigned a value) and false otherwise */
  public boolean isSetKin() {
    return this.kin != null;
  }

  public void setKinIsSet(boolean value) {
    if (!value) {
      this.kin = null;
    }
  }

  public void setFieldValue(_Fields field, Object value) {
    switch (field) {
    case LIMB:
      if (value == null) {
        unsetLimb();
      } else {
        setLimb((Limb)value);
      }
      break;

    case INITIAL_POS:
      if (value == null) {
        unsetInitial_pos();
      } else {
        setInitial_pos((Position)value);
      }
      break;

    case FINAL_POS:
      if (value == null) {
        unsetFinal_pos();
      } else {
        setFinal_pos((Position)value);
      }
      break;

    case INITIAL_ORI:
      if (value == null) {
        unsetInitial_ori();
      } else {
        setInitial_ori((Orientation)value);
      }
      break;

    case FINAL_ORI:
      if (value == null) {
        unsetFinal_ori();
      } else {
        setFinal_ori((Orientation)value);
      }
      break;

    case SPEED:
      if (value == null) {
        unsetSpeed();
      } else {
        setSpeed((Speed)value);
      }
      break;

    case ANGLS:
      if (value == null) {
        unsetAngls();
      } else {
        setAngls((Angles)value);
      }
      break;

    case MODE:
      if (value == null) {
        unsetMode();
      } else {
        setMode((Reference_sys)value);
      }
      break;

    case KIN:
      if (value == null) {
        unsetKin();
      } else {
        setKin((Kinematics)value);
      }
      break;

    }
  }

  public Object getFieldValue(_Fields field) {
    switch (field) {
    case LIMB:
      return getLimb();

    case INITIAL_POS:
      return getInitial_pos();

    case FINAL_POS:
      return getFinal_pos();

    case INITIAL_ORI:
      return getInitial_ori();

    case FINAL_ORI:
      return getFinal_ori();

    case SPEED:
      return getSpeed();

    case ANGLS:
      return getAngls();

    case MODE:
      return getMode();

    case KIN:
      return getKin();

    }
    throw new IllegalStateException();
  }

  /** Returns true if field corresponding to fieldID is set (has been assigned a value) and false otherwise */
  public boolean isSet(_Fields field) {
    if (field == null) {
      throw new IllegalArgumentException();
    }

    switch (field) {
    case LIMB:
      return isSetLimb();
    case INITIAL_POS:
      return isSetInitial_pos();
    case FINAL_POS:
      return isSetFinal_pos();
    case INITIAL_ORI:
      return isSetInitial_ori();
    case FINAL_ORI:
      return isSetFinal_ori();
    case SPEED:
      return isSetSpeed();
    case ANGLS:
      return isSetAngls();
    case MODE:
      return isSetMode();
    case KIN:
      return isSetKin();
    }
    throw new IllegalStateException();
  }

  @Override
  public boolean equals(Object that) {
    if (that == null)
      return false;
    if (that instanceof PickAndPlaceEvent)
      return this.equals((PickAndPlaceEvent)that);
    return false;
  }

  public boolean equals(PickAndPlaceEvent that) {
    if (that == null)
      return false;

    boolean this_present_limb = true && this.isSetLimb();
    boolean that_present_limb = true && that.isSetLimb();
    if (this_present_limb || that_present_limb) {
      if (!(this_present_limb && that_present_limb))
        return false;
      if (!this.limb.equals(that.limb))
        return false;
    }

    boolean this_present_initial_pos = true && this.isSetInitial_pos();
    boolean that_present_initial_pos = true && that.isSetInitial_pos();
    if (this_present_initial_pos || that_present_initial_pos) {
      if (!(this_present_initial_pos && that_present_initial_pos))
        return false;
      if (!this.initial_pos.equals(that.initial_pos))
        return false;
    }

    boolean this_present_final_pos = true && this.isSetFinal_pos();
    boolean that_present_final_pos = true && that.isSetFinal_pos();
    if (this_present_final_pos || that_present_final_pos) {
      if (!(this_present_final_pos && that_present_final_pos))
        return false;
      if (!this.final_pos.equals(that.final_pos))
        return false;
    }

    boolean this_present_initial_ori = true && this.isSetInitial_ori();
    boolean that_present_initial_ori = true && that.isSetInitial_ori();
    if (this_present_initial_ori || that_present_initial_ori) {
      if (!(this_present_initial_ori && that_present_initial_ori))
        return false;
      if (!this.initial_ori.equals(that.initial_ori))
        return false;
    }

    boolean this_present_final_ori = true && this.isSetFinal_ori();
    boolean that_present_final_ori = true && that.isSetFinal_ori();
    if (this_present_final_ori || that_present_final_ori) {
      if (!(this_present_final_ori && that_present_final_ori))
        return false;
      if (!this.final_ori.equals(that.final_ori))
        return false;
    }

    boolean this_present_speed = true && this.isSetSpeed();
    boolean that_present_speed = true && that.isSetSpeed();
    if (this_present_speed || that_present_speed) {
      if (!(this_present_speed && that_present_speed))
        return false;
      if (!this.speed.equals(that.speed))
        return false;
    }

    boolean this_present_angls = true && this.isSetAngls();
    boolean that_present_angls = true && that.isSetAngls();
    if (this_present_angls || that_present_angls) {
      if (!(this_present_angls && that_present_angls))
        return false;
      if (!this.angls.equals(that.angls))
        return false;
    }

    boolean this_present_mode = true && this.isSetMode();
    boolean that_present_mode = true && that.isSetMode();
    if (this_present_mode || that_present_mode) {
      if (!(this_present_mode && that_present_mode))
        return false;
      if (!this.mode.equals(that.mode))
        return false;
    }

    boolean this_present_kin = true && this.isSetKin();
    boolean that_present_kin = true && that.isSetKin();
    if (this_present_kin || that_present_kin) {
      if (!(this_present_kin && that_present_kin))
        return false;
      if (!this.kin.equals(that.kin))
        return false;
    }

    return true;
  }

  @Override
  public int hashCode() {
    return 0;
  }

  public int compareTo(PickAndPlaceEvent other) {
    if (!getClass().equals(other.getClass())) {
      return getClass().getName().compareTo(other.getClass().getName());
    }

    int lastComparison = 0;
    PickAndPlaceEvent typedOther = (PickAndPlaceEvent)other;

    lastComparison = Boolean.valueOf(isSetLimb()).compareTo(typedOther.isSetLimb());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetLimb()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.limb, typedOther.limb);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetInitial_pos()).compareTo(typedOther.isSetInitial_pos());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetInitial_pos()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.initial_pos, typedOther.initial_pos);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetFinal_pos()).compareTo(typedOther.isSetFinal_pos());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetFinal_pos()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.final_pos, typedOther.final_pos);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetInitial_ori()).compareTo(typedOther.isSetInitial_ori());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetInitial_ori()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.initial_ori, typedOther.initial_ori);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetFinal_ori()).compareTo(typedOther.isSetFinal_ori());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetFinal_ori()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.final_ori, typedOther.final_ori);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetSpeed()).compareTo(typedOther.isSetSpeed());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetSpeed()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.speed, typedOther.speed);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetAngls()).compareTo(typedOther.isSetAngls());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetAngls()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.angls, typedOther.angls);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetMode()).compareTo(typedOther.isSetMode());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetMode()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.mode, typedOther.mode);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    lastComparison = Boolean.valueOf(isSetKin()).compareTo(typedOther.isSetKin());
    if (lastComparison != 0) {
      return lastComparison;
    }
    if (isSetKin()) {
      lastComparison = org.apache.thrift.TBaseHelper.compareTo(this.kin, typedOther.kin);
      if (lastComparison != 0) {
        return lastComparison;
      }
    }
    return 0;
  }

  public _Fields fieldForId(int fieldId) {
    return _Fields.findByThriftId(fieldId);
  }

  public void read(org.apache.thrift.protocol.TProtocol iprot) throws org.apache.thrift.TException {
    schemes.get(iprot.getScheme()).getScheme().read(iprot, this);
  }

  public void write(org.apache.thrift.protocol.TProtocol oprot) throws org.apache.thrift.TException {
    schemes.get(oprot.getScheme()).getScheme().write(oprot, this);
  }

  @Override
  public String toString() {
    StringBuilder sb = new StringBuilder("PickAndPlaceEvent(");
    boolean first = true;

    sb.append("limb:");
    if (this.limb == null) {
      sb.append("null");
    } else {
      sb.append(this.limb);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("initial_pos:");
    if (this.initial_pos == null) {
      sb.append("null");
    } else {
      sb.append(this.initial_pos);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("final_pos:");
    if (this.final_pos == null) {
      sb.append("null");
    } else {
      sb.append(this.final_pos);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("initial_ori:");
    if (this.initial_ori == null) {
      sb.append("null");
    } else {
      sb.append(this.initial_ori);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("final_ori:");
    if (this.final_ori == null) {
      sb.append("null");
    } else {
      sb.append(this.final_ori);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("speed:");
    if (this.speed == null) {
      sb.append("null");
    } else {
      sb.append(this.speed);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("angls:");
    if (this.angls == null) {
      sb.append("null");
    } else {
      sb.append(this.angls);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("mode:");
    if (this.mode == null) {
      sb.append("null");
    } else {
      sb.append(this.mode);
    }
    first = false;
    if (!first) sb.append(", ");
    sb.append("kin:");
    if (this.kin == null) {
      sb.append("null");
    } else {
      sb.append(this.kin);
    }
    first = false;
    sb.append(")");
    return sb.toString();
  }

  public void validate() throws org.apache.thrift.TException {
    // check for required fields
    if (limb == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'limb' was not present! Struct: " + toString());
    }
    if (initial_pos == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'initial_pos' was not present! Struct: " + toString());
    }
    if (final_pos == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'final_pos' was not present! Struct: " + toString());
    }
    if (initial_ori == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'initial_ori' was not present! Struct: " + toString());
    }
    if (final_ori == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'final_ori' was not present! Struct: " + toString());
    }
    if (speed == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'speed' was not present! Struct: " + toString());
    }
    if (angls == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'angls' was not present! Struct: " + toString());
    }
    if (mode == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'mode' was not present! Struct: " + toString());
    }
    if (kin == null) {
      throw new org.apache.thrift.protocol.TProtocolException("Required field 'kin' was not present! Struct: " + toString());
    }
    // check for sub-struct validity
    if (initial_pos != null) {
      initial_pos.validate();
    }
    if (final_pos != null) {
      final_pos.validate();
    }
    if (initial_ori != null) {
      initial_ori.validate();
    }
    if (final_ori != null) {
      final_ori.validate();
    }
    if (speed != null) {
      speed.validate();
    }
    if (angls != null) {
      angls.validate();
    }
  }

  private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException {
    try {
      write(new org.apache.thrift.protocol.TCompactProtocol(new org.apache.thrift.transport.TIOStreamTransport(out)));
    } catch (org.apache.thrift.TException te) {
      throw new java.io.IOException(te);
    }
  }

  private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException {
    try {
      read(new org.apache.thrift.protocol.TCompactProtocol(new org.apache.thrift.transport.TIOStreamTransport(in)));
    } catch (org.apache.thrift.TException te) {
      throw new java.io.IOException(te);
    }
  }

  private static class PickAndPlaceEventStandardSchemeFactory implements SchemeFactory {
    public PickAndPlaceEventStandardScheme getScheme() {
      return new PickAndPlaceEventStandardScheme();
    }
  }

  private static class PickAndPlaceEventStandardScheme extends StandardScheme<PickAndPlaceEvent> {

    public void read(org.apache.thrift.protocol.TProtocol iprot, PickAndPlaceEvent struct) throws org.apache.thrift.TException {
      org.apache.thrift.protocol.TField schemeField;
      iprot.readStructBegin();
      while (true)
      {
        schemeField = iprot.readFieldBegin();
        if (schemeField.type == org.apache.thrift.protocol.TType.STOP) { 
          break;
        }
        switch (schemeField.id) {
          case 1: // LIMB
            if (schemeField.type == org.apache.thrift.protocol.TType.I32) {
              struct.limb = Limb.findByValue(iprot.readI32());
              struct.setLimbIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 2: // INITIAL_POS
            if (schemeField.type == org.apache.thrift.protocol.TType.STRUCT) {
              struct.initial_pos = new Position();
              struct.initial_pos.read(iprot);
              struct.setInitial_posIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 3: // FINAL_POS
            if (schemeField.type == org.apache.thrift.protocol.TType.STRUCT) {
              struct.final_pos = new Position();
              struct.final_pos.read(iprot);
              struct.setFinal_posIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 4: // INITIAL_ORI
            if (schemeField.type == org.apache.thrift.protocol.TType.STRUCT) {
              struct.initial_ori = new Orientation();
              struct.initial_ori.read(iprot);
              struct.setInitial_oriIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 5: // FINAL_ORI
            if (schemeField.type == org.apache.thrift.protocol.TType.STRUCT) {
              struct.final_ori = new Orientation();
              struct.final_ori.read(iprot);
              struct.setFinal_oriIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 6: // SPEED
            if (schemeField.type == org.apache.thrift.protocol.TType.STRUCT) {
              struct.speed = new Speed();
              struct.speed.read(iprot);
              struct.setSpeedIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 7: // ANGLS
            if (schemeField.type == org.apache.thrift.protocol.TType.STRUCT) {
              struct.angls = new Angles();
              struct.angls.read(iprot);
              struct.setAnglsIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 8: // MODE
            if (schemeField.type == org.apache.thrift.protocol.TType.I32) {
              struct.mode = Reference_sys.findByValue(iprot.readI32());
              struct.setModeIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          case 9: // KIN
            if (schemeField.type == org.apache.thrift.protocol.TType.I32) {
              struct.kin = Kinematics.findByValue(iprot.readI32());
              struct.setKinIsSet(true);
            } else { 
              org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
            }
            break;
          default:
            org.apache.thrift.protocol.TProtocolUtil.skip(iprot, schemeField.type);
        }
        iprot.readFieldEnd();
      }
      iprot.readStructEnd();

      // check for required fields of primitive type, which can't be checked in the validate method
      struct.validate();
    }

    public void write(org.apache.thrift.protocol.TProtocol oprot, PickAndPlaceEvent struct) throws org.apache.thrift.TException {
      struct.validate();

      oprot.writeStructBegin(STRUCT_DESC);
      if (struct.limb != null) {
        oprot.writeFieldBegin(LIMB_FIELD_DESC);
        oprot.writeI32(struct.limb.getValue());
        oprot.writeFieldEnd();
      }
      if (struct.initial_pos != null) {
        oprot.writeFieldBegin(INITIAL_POS_FIELD_DESC);
        struct.initial_pos.write(oprot);
        oprot.writeFieldEnd();
      }
      if (struct.final_pos != null) {
        oprot.writeFieldBegin(FINAL_POS_FIELD_DESC);
        struct.final_pos.write(oprot);
        oprot.writeFieldEnd();
      }
      if (struct.initial_ori != null) {
        oprot.writeFieldBegin(INITIAL_ORI_FIELD_DESC);
        struct.initial_ori.write(oprot);
        oprot.writeFieldEnd();
      }
      if (struct.final_ori != null) {
        oprot.writeFieldBegin(FINAL_ORI_FIELD_DESC);
        struct.final_ori.write(oprot);
        oprot.writeFieldEnd();
      }
      if (struct.speed != null) {
        oprot.writeFieldBegin(SPEED_FIELD_DESC);
        struct.speed.write(oprot);
        oprot.writeFieldEnd();
      }
      if (struct.angls != null) {
        oprot.writeFieldBegin(ANGLS_FIELD_DESC);
        struct.angls.write(oprot);
        oprot.writeFieldEnd();
      }
      if (struct.mode != null) {
        oprot.writeFieldBegin(MODE_FIELD_DESC);
        oprot.writeI32(struct.mode.getValue());
        oprot.writeFieldEnd();
      }
      if (struct.kin != null) {
        oprot.writeFieldBegin(KIN_FIELD_DESC);
        oprot.writeI32(struct.kin.getValue());
        oprot.writeFieldEnd();
      }
      oprot.writeFieldStop();
      oprot.writeStructEnd();
    }

  }

  private static class PickAndPlaceEventTupleSchemeFactory implements SchemeFactory {
    public PickAndPlaceEventTupleScheme getScheme() {
      return new PickAndPlaceEventTupleScheme();
    }
  }

  private static class PickAndPlaceEventTupleScheme extends TupleScheme<PickAndPlaceEvent> {

    @Override
    public void write(org.apache.thrift.protocol.TProtocol prot, PickAndPlaceEvent struct) throws org.apache.thrift.TException {
      TTupleProtocol oprot = (TTupleProtocol) prot;
      oprot.writeI32(struct.limb.getValue());
      struct.initial_pos.write(oprot);
      struct.final_pos.write(oprot);
      struct.initial_ori.write(oprot);
      struct.final_ori.write(oprot);
      struct.speed.write(oprot);
      struct.angls.write(oprot);
      oprot.writeI32(struct.mode.getValue());
      oprot.writeI32(struct.kin.getValue());
    }

    @Override
    public void read(org.apache.thrift.protocol.TProtocol prot, PickAndPlaceEvent struct) throws org.apache.thrift.TException {
      TTupleProtocol iprot = (TTupleProtocol) prot;
      struct.limb = Limb.findByValue(iprot.readI32());
      struct.setLimbIsSet(true);
      struct.initial_pos = new Position();
      struct.initial_pos.read(iprot);
      struct.setInitial_posIsSet(true);
      struct.final_pos = new Position();
      struct.final_pos.read(iprot);
      struct.setFinal_posIsSet(true);
      struct.initial_ori = new Orientation();
      struct.initial_ori.read(iprot);
      struct.setInitial_oriIsSet(true);
      struct.final_ori = new Orientation();
      struct.final_ori.read(iprot);
      struct.setFinal_oriIsSet(true);
      struct.speed = new Speed();
      struct.speed.read(iprot);
      struct.setSpeedIsSet(true);
      struct.angls = new Angles();
      struct.angls.read(iprot);
      struct.setAnglsIsSet(true);
      struct.mode = Reference_sys.findByValue(iprot.readI32());
      struct.setModeIsSet(true);
      struct.kin = Kinematics.findByValue(iprot.readI32());
      struct.setKinIsSet(true);
    }
  }

}

