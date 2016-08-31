/**
 * Autogenerated by Thrift Compiler (0.9.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
package de.dfki.tecs.robot.baxter;


import java.util.Map;
import java.util.HashMap;
import org.apache.thrift.TEnum;

public enum Kinematics implements org.apache.thrift.TEnum {
  INVERSE(0),
  FORWARD(1);

  private final int value;

  private Kinematics(int value) {
    this.value = value;
  }

  /**
   * Get the integer value of this enum value, as defined in the Thrift IDL.
   */
  public int getValue() {
    return value;
  }

  /**
   * Find a the enum type by its integer value, as defined in the Thrift IDL.
   * @return null if the value is not found.
   */
  public static Kinematics findByValue(int value) { 
    switch (value) {
      case 0:
        return INVERSE;
      case 1:
        return FORWARD;
      default:
        return null;
    }
  }
}
