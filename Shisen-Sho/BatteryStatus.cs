using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Shisen_Sho {

	public class SYSTEM_POWER_STATUS_EX2 {
		public byte ACLineStatus;
		public byte BatteryFlag;
		public byte BatteryLifePercent;
		public byte Reserved1;
		public uint BatteryLifeTime;
		public uint BatteryFullLifeTime;
		public byte Reserved2;
		public byte BackupBatteryFlag;
		public byte BackupBatteryLifePercent;
		public byte Reserved3;
		public uint BackupBatteryLifeTime;
		public uint BackupBatteryFullLifeTime;
		public uint BatteryVoltage;
		public uint BatteryCurrent;
		public uint BateryAverageCurrent;
		public uint BatteryAverageInterval;
		public uint BatterymAHourConsumed;
		public uint BatteryTemperature;
		public uint BackupBatteryVoltage;
		public byte BatteryChemistry;
	}

	public class SYSTEM_POWER_STATUS_EX {
		public byte ACLineStatus;
		public byte BatteryFlag;
		public byte BatteryLifePercent;
		public byte Reserved1;
		public uint BatteryLifeTime;
		public uint BatteryFullLifeTime;
		public byte Reserved2;
		public byte BackupBatteryFlag;
		public byte BackupBatteryLifePercent;
		public uint BackupBatteryLifeTime;
		public uint BackupBatteryFullLifeTime;
	}

	public class BatteryStatus {
		// returs 1 if good
		[DllImport("coredll")]
		private static extern uint GetSystemPowerStatusEx(SYSTEM_POWER_STATUS_EX lpSystemPowerStatus, bool fUpdate);


		// returns non-zero if good
		// dwLen should be passed (uint)Marshal.GetSize(status)
		[DllImport("coredll")]
		private static extern uint GetSystemPowerStatusEx2(SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, uint dwLen, bool fUpdate);

		public static string BatteryLifePercent() {
			SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();

			if (GetSystemPowerStatusEx(status, false) != 0) {
				return String.Format("{0}%", status.BatteryLifePercent);
			} else {
				return "??";
			}
		}

		public static int BatteryLifePercentInt() {
			SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();

			if (GetSystemPowerStatusEx(status, false) != 0) {
				return (int)status.BatteryLifePercent;
			} else {
				return 255;
			}
		}

		public static int BatteryFlag() {
			SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();

			if (GetSystemPowerStatusEx(status, false) != 0) {
				return status.BatteryFlag;
			} else {
				return 0;
			}
		}

		public static string ACPowerStatus() {
			SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();

			if (GetSystemPowerStatusEx(status, false) != 0) {
				return String.Format("AC: {0}", status.ACLineStatus);
			} else {
				return "AC: ?";
			}
		}

		public static bool OnCharge() {
			int flags = BatteryStatus.BatteryFlag();
			if ((flags & 8) != 0) {
				if (BatteryStatus.BatteryLifePercentInt() > 200) return false;
				return true;
			}
			return false;
		}

		public static string BatteryTime() {
			return "Unknown";
		}
	}
}
