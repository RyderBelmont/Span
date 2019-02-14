﻿namespace KisekaeImporter
{
	public enum KisekaePart
	{
		None,
		Head,
		Mouth,
		Nose,
		Face,
		LeftEar,
		RightEar,
		LeftEye,
		RightEye,
		UpperBody,
		LowerBody,
		LeftLeg,
		RightLeg,
		LeftUpperArmShoulder,
		LeftLowerArmHand,
		RightUpperArmShoulder,
		RightLowerArmHand,
		Penis,
		Vibrator,
		UpperJacket,
		LowerJacket,
		UpperVest,
		LowerVest,
		UpperShirt,
		LowerShirt,
		Necktie,
		Bra,
		Panties,
		Skirt,
	}

	public static class KisekaeExtensions
	{
		public static string ToDisplayName(this KisekaePart part)
		{
			switch (part)
			{
				case KisekaePart.Head:
					return "Head";
				case KisekaePart.Mouth:
					return "Mouth";
				case KisekaePart.Nose:
					return "Nose";
				case KisekaePart.Face:
					return "Face";
				case KisekaePart.LeftEar:
					return "Left ear";
				case KisekaePart.RightEar:
					return "Right ear";
				case KisekaePart.LeftEye:
					return "Left eye";
				case KisekaePart.RightEye:
					return "Right eye";
				case KisekaePart.UpperBody:
					return "Body (upper)";
				case KisekaePart.LowerBody:
					return "Body (lower)";
				case KisekaePart.LeftLeg:
					return "Left leg";
				case KisekaePart.RightLeg:
					return "Right leg";
				case KisekaePart.LeftUpperArmShoulder:
					return "Left upper arm";
				case KisekaePart.LeftLowerArmHand:
					return "Left forearm/hand";
				case KisekaePart.RightUpperArmShoulder:
					return "Right upper arm";
				case KisekaePart.RightLowerArmHand:
					return "Right forearm/hand";
				case KisekaePart.Penis:
					return "Penis";
				case KisekaePart.Vibrator:
					return "Vibrator";
				case KisekaePart.UpperJacket:
					return "Jacket (upper)";
				case KisekaePart.LowerJacket:
					return "Jacket (lower)";
				case KisekaePart.UpperVest:
					return "Vest (upper)";
				case KisekaePart.LowerVest:
					return "Vest (lower)";
				case KisekaePart.UpperShirt:
					return "Shirt (upper)";
				case KisekaePart.LowerShirt:
					return "Shirt (lower)";
				case KisekaePart.None:
					return "None";
				default:
					return part.ToString();
			}
		}

		public static string Serialize(this KisekaePart part)
		{
			switch (part)
			{
				case KisekaePart.Head:
					return "head";
				case KisekaePart.Mouth:
					return "head.mouth";
				case KisekaePart.Nose:
					return "head.nose";
				case KisekaePart.Face:
					return "head.face";
				case KisekaePart.LeftEar:
					return "head.ear1";
				case KisekaePart.RightEar:
					return "head.ear0";
				case KisekaePart.LeftEye:
					return "head.eye.eye1";
				case KisekaePart.RightEye:
					return "head.eye.eye0";
				case KisekaePart.UpperBody:
					return "mune";
				case KisekaePart.LowerBody:
					return "dou";
				case KisekaePart.LeftLeg:
					return "ashi1";
				case KisekaePart.RightLeg:
					return "ashi0";
				case KisekaePart.LeftUpperArmShoulder:
					return "handm0_1";
				case KisekaePart.LeftLowerArmHand:
					return "handm1_1";
				case KisekaePart.RightUpperArmShoulder:
					return "handm0_0";
				case KisekaePart.RightLowerArmHand:
					return "handm1_0";
				case KisekaePart.Penis:
					return "peni";
				case KisekaePart.Vibrator:
					return "vibrator";
				case KisekaePart.UpperJacket:
					return "mune.SeihukuMune";
				case KisekaePart.LowerJacket:
					return "dou.SeihukuDou";
				case KisekaePart.UpperVest:
					return "mune.VestMune/mune.VestMune2";
				case KisekaePart.LowerVest:
					return "dou.VestDou";
				case KisekaePart.UpperShirt:
					return "mune.YsyatuMune/mune.YsyatuMune2";
				case KisekaePart.LowerShirt:
					return "dou.YsyatuDou";
				case KisekaePart.Necktie:
					return "mune.Necktie0/mune.Necktie1";
				case KisekaePart.Bra:
					return "mune.Bura/dou.Bura";
				case KisekaePart.Panties:
					return "dou.dou_shitaHuku.Pantu";
				case KisekaePart.Skirt:
					return "dou.dou_Skirt.Skirt";
				default:
					return "";
			}
		}

		public static KisekaePart ToKisekaePart(this string part)
		{
			switch (part)
			{
				case "head":
					return KisekaePart.Head;
				case "head.mouth":
					return KisekaePart.Mouth;
				case "head.nose":
					return KisekaePart.Nose;
				case "head.face":
					return KisekaePart.Face;
				case "head.ear1":
					return KisekaePart.LeftEar;
				case "head.ear0":
					return KisekaePart.RightEar;
				case "head.eye.eye1":
					return KisekaePart.LeftEye;
				case "head.eye.eye0":
					return KisekaePart.RightEye;
				case "mune":
					return KisekaePart.UpperBody;
				case "dou":
					return KisekaePart.LowerBody;
				case "ashi1":
					return KisekaePart.LeftLeg;
				case "ashi0":
					return KisekaePart.RightLeg;
				case "handm0_1":
					return KisekaePart.LeftUpperArmShoulder;
				case "handm1_1":
					return KisekaePart.LeftLowerArmHand;
				case "handm0_0":
					return KisekaePart.RightUpperArmShoulder;
				case "handm1_0":
					return KisekaePart.RightLowerArmHand;
				case "peni":
					return KisekaePart.Penis;
				case "vibrator":
					return KisekaePart.Vibrator;
				case "mune.SeihukuMune":
					return KisekaePart.UpperJacket;
				case "dou.SeihukuDou":
					return KisekaePart.LowerJacket;
				case "mune.VestMune/mune.VestMune2":
					return KisekaePart.UpperVest;
				case "dou.VestDou":
					return KisekaePart.LowerVest;
				case "mune.YsyatuMune/mune.YsyatuMune2":
					return KisekaePart.UpperShirt;
				case "dou.YsyatuDou":
					return KisekaePart.LowerShirt;
				case "mune.Necktie0/mune.Necktie1":
					return KisekaePart.Necktie;
				case "mune.Bura/dou.Bura":
					return KisekaePart.Bra;
				case "dou.dou_shitaHuku.Pantu":
					return KisekaePart.Panties;
				case "dou.dou_Skirt.Skirt":
					return KisekaePart.Skirt;
				default:
					return KisekaePart.None;
			}
		}
	}
}
