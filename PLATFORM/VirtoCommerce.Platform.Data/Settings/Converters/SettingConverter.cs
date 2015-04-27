﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Model;
using System.Collections;


namespace VirtoCommerce.Platform.Data.Settings.Converters
{
	public static class SettingConverter
	{
		public static SettingDescriptor ToModel(this ModuleSetting moduleSetting, SettingEntity entity)
		{
		
			var retVal = new SettingDescriptor();
			retVal.InjectFrom(moduleSetting);
			retVal.Value = moduleSetting.DefaultValue;
			retVal.ValueType = ConvertToSettingValueType(moduleSetting.ValueType);

			if (entity != null)
			{
				var existingValues = entity.SettingValues.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

				if (moduleSetting.IsArray)
				{
					retVal.ArrayValues = existingValues;
				}
				else
				{
					if (existingValues.Any())
					{
						retVal.Value = existingValues.First();
					}
				}
			}

			return retVal;
		}

		public static SettingDescriptor ToModel<T>(this string settingName, T value)
		{
			var type = typeof(T);
			var retVal = new SettingDescriptor { Name = settingName };

			if (type.IsArray)
			{
				retVal.IsArray = true;
				retVal.ValueType = ConvertToSettingValueType(type.GetElementType());

				if (value != null)
				{
					retVal.ArrayValues = ((IEnumerable)value).OfType<object>()
										.Select(v => v == null ? null : string.Format(CultureInfo.InvariantCulture, "{0}", v))
										.ToArray();
				}
			}
			else
			{
				retVal.ValueType = ConvertToSettingValueType(type);
				retVal.Value = value == null ? null : string.Format(CultureInfo.InvariantCulture, "{0}", value);
			}
			return retVal;
		}

		public static SettingEntity ToEntity(this SettingDescriptor setting)
		{
			if (setting == null)
				throw new ArgumentNullException("setting");

			var retVal = new SettingEntity();
			retVal.InjectFrom(setting);
			retVal.SettingValueType = setting.ValueType.ToString();
			retVal.IsEnum = setting.IsArray;

			var valueEntities = new List<string>();
			if(setting.ArrayValues != null)
			{
				valueEntities.AddRange(setting.ArrayValues);
			} 
			if(setting.Value != null)
			{
				valueEntities.Add(setting.Value);
			}

			retVal.SettingValues = new ObservableCollection<SettingValueEntity>(valueEntities.Select(x => x.ToEntity(setting.ValueType)));
		
			return retVal;
		}

		/// <summary>
		/// Patch CatalogBase type
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static void Patch(this SettingEntity source, SettingEntity target)
		{
			if (target == null)
				throw new ArgumentNullException("target");

			if (!source.SettingValues.IsNullCollection())
			{
				var comparer = AnonymousComparer.Create((SettingValueEntity x) => x.ToString(CultureInfo.InvariantCulture));
				source.SettingValues.Patch(target.SettingValues, comparer, (sourceSetting, targetSetting) => { return; });
			}
		}

		private static SettingValueType ConvertToSettingValueType(string valueType)
		{
			var retVal = SettingValueType.ShortText;
			if (string.Equals(valueType, ModuleSetting.TypeBoolean, StringComparison.InvariantCultureIgnoreCase))
			{
				retVal = SettingValueType.Boolean;
			}
			else if (string.Equals(valueType, ModuleSetting.TypeInteger, StringComparison.InvariantCultureIgnoreCase))
			{
				retVal = SettingValueType.Integrer;
			}
			else if (string.Equals(valueType, ModuleSetting.TypeDecimal, StringComparison.InvariantCultureIgnoreCase))
			{
				retVal = SettingValueType.Decimal;
			}
			else if (string.Equals(valueType, ModuleSetting.TypeSecureString, StringComparison.InvariantCultureIgnoreCase))
			{
				retVal = SettingValueType.SecureString;
			}
		
			return retVal;
		}


		private static SettingValueType ConvertToSettingValueType(Type valueType)
		{
			var retVal = SettingValueType.ShortText;
			if(valueType == typeof(bool))
			{
				retVal = SettingValueType.Boolean;
			}
			else if(valueType == typeof(Int32))
			{
				retVal = SettingValueType.Integrer;
			}
			else if(valueType == typeof(decimal))
			{
				retVal = SettingValueType.Decimal;
			}
			return retVal;
		}

	}
}