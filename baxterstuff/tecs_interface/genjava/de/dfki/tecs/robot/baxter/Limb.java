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

public enum Limb implements org.apache.thrift.TEnum {
  BOTH(0),
  LEFT(1),
  RIGHT(2);

  private final int value;

  private Limb(int value) {
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
  public static Limb findByValue(int value) { 
    switch (value) {
      case 0:
        return BOTH;
      case 1:
        return LEFT;
      case 2:
        return RIGHT;
      default:
        return null;
    }
  }
}
