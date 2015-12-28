#region Copyright
////////////////////////////////////////////////////////////////////////////////
// The following FIT Protocol software provided may be used with FIT protocol
// devices only and remains the copyrighted property of Dynastream Innovations Inc.
// The software is being provided on an "as-is" basis and as an accommodation,
// and therefore all warranties, representations, or guarantees of any kind
// (whether express, implied or statutory) including, without limitation,
// warranties of merchantability, non-infringement, or fitness for a particular
// purpose, are specifically disclaimed.
//
// Copyright 2015 Dynastream Innovations Inc.
////////////////////////////////////////////////////////////////////////////////
// ****WARNING****  This file is auto-generated!  Do NOT edit this file.
// Profile Version = 16.10Release
// Tag = development-akw-16.10.00-0
////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;


namespace FickleFrostbite.FIT
{
   /// <summary>
   /// Implements the ZonesTarget profile message.
   /// </summary>
   public class ZonesTargetMesg : Mesg
   {
      #region Fields
      #endregion

      #region Constructors
      public ZonesTargetMesg() : base(Profile.mesgs[Profile.ZonesTargetIndex])
      {
      }

      public ZonesTargetMesg(Mesg mesg) : base(mesg)
      {
      }
      #endregion // Constructors

      #region Methods
      ///<summary>
      /// Retrieves the MaxHeartRate field</summary>
      /// <returns>Returns nullable byte representing the MaxHeartRate field</returns>
      public byte? GetMaxHeartRate()
      {
         return (byte?)GetFieldValue(1, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set MaxHeartRate field</summary>
      /// <param name="maxHeartRate_">Nullable field value to be set</param>
      public void SetMaxHeartRate(byte? maxHeartRate_)
      {
         SetFieldValue(1, 0, maxHeartRate_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the ThresholdHeartRate field</summary>
      /// <returns>Returns nullable byte representing the ThresholdHeartRate field</returns>
      public byte? GetThresholdHeartRate()
      {
         return (byte?)GetFieldValue(2, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set ThresholdHeartRate field</summary>
      /// <param name="thresholdHeartRate_">Nullable field value to be set</param>
      public void SetThresholdHeartRate(byte? thresholdHeartRate_)
      {
         SetFieldValue(2, 0, thresholdHeartRate_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the FunctionalThresholdPower field</summary>
      /// <returns>Returns nullable ushort representing the FunctionalThresholdPower field</returns>
      public ushort? GetFunctionalThresholdPower()
      {
         return (ushort?)GetFieldValue(3, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set FunctionalThresholdPower field</summary>
      /// <param name="functionalThresholdPower_">Nullable field value to be set</param>
      public void SetFunctionalThresholdPower(ushort? functionalThresholdPower_)
      {
         SetFieldValue(3, 0, functionalThresholdPower_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the HrCalcType field</summary>
      /// <returns>Returns nullable HrZoneCalc enum representing the HrCalcType field</returns>
      public HrZoneCalc? GetHrCalcType()
      {
         object obj = GetFieldValue(5, 0, Fit.SubfieldIndexMainField);
         HrZoneCalc? value = obj == null ? (HrZoneCalc?)null : (HrZoneCalc)obj;
         return value;
      }

      

      

      /// <summary>
      /// Set HrCalcType field</summary>
      /// <param name="hrCalcType_">Nullable field value to be set</param>
      public void SetHrCalcType(HrZoneCalc? hrCalcType_)
      {
         SetFieldValue(5, 0, hrCalcType_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the PwrCalcType field</summary>
      /// <returns>Returns nullable PwrZoneCalc enum representing the PwrCalcType field</returns>
      public PwrZoneCalc? GetPwrCalcType()
      {
         object obj = GetFieldValue(7, 0, Fit.SubfieldIndexMainField);
         PwrZoneCalc? value = obj == null ? (PwrZoneCalc?)null : (PwrZoneCalc)obj;
         return value;
      }

      

      

      /// <summary>
      /// Set PwrCalcType field</summary>
      /// <param name="pwrCalcType_">Nullable field value to be set</param>
      public void SetPwrCalcType(PwrZoneCalc? pwrCalcType_)
      {
         SetFieldValue(7, 0, pwrCalcType_, Fit.SubfieldIndexMainField);
      }
      
      #endregion // Methods
   } // Class
} // namespace
