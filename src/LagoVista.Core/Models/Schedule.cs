// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cc8e2126a89915c261045a95d21acd3253435cc53283e1a482b4b6db62d5aeaa
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{

    public enum WeekNumbers
    {
        [EnumLabel(Schedule.FirstWeek, LagoVistaCommonStrings.Names.Schedule_FirstWeek, typeof(LagoVistaCommonStrings))]
        FirstWeek,
        [EnumLabel(Schedule.SecondWeek, LagoVistaCommonStrings.Names.Schedule_SecondWeek, typeof(LagoVistaCommonStrings))]
        SecondWeek,
        [EnumLabel(Schedule.ThirdWeek, LagoVistaCommonStrings.Names.Schedule_ThirdWeek, typeof(LagoVistaCommonStrings))]
        ThirdWeek,
        [EnumLabel(Schedule.FourthWeek, LagoVistaCommonStrings.Names.Schedule_FourthWeek, typeof(LagoVistaCommonStrings))]
        FourthWeek
    }

    public enum ScheduleTypes
    {
        [EnumLabel(Schedule.Daily, LagoVistaCommonStrings.Names.Schedule_ScheduleType_Daily, typeof(LagoVistaCommonStrings))]
        Daily,
        [EnumLabel(Schedule.Weekly, LagoVistaCommonStrings.Names.Schedule_ScheduleType_Weekly, typeof(LagoVistaCommonStrings))]
        Weekly,
        [EnumLabel(Schedule.Biweekly, LagoVistaCommonStrings.Names.Schedule_ScheduleType_Biweekly, typeof(LagoVistaCommonStrings))]
        Biweekly,
        [EnumLabel(Schedule.Monthly, LagoVistaCommonStrings.Names.Schedule_ScheduleType_Monthly, typeof(LagoVistaCommonStrings))]
        Monthly,
        [EnumLabel(Schedule.FirstDayOfMonth, LagoVistaCommonStrings.Names.Schedule_ScheduleType_FirstDayOfMonth, typeof(LagoVistaCommonStrings))]
        FirstDayOfMonth,
        [EnumLabel(Schedule.LastDayOfMonth, LagoVistaCommonStrings.Names.Schedule_ScheduleType_LastDayOfMonth, typeof(LagoVistaCommonStrings))]
        LaststDayOfMonth
    }

    public enum WeekDays
    {
        [EnumLabel(Schedule.StrSunday, LagoVistaCommonStrings.Names.Schedule_Sunday, typeof(LagoVistaCommonStrings))]
        Sunday,
        [EnumLabel(Schedule.StrMonday, LagoVistaCommonStrings.Names.Schedule_Monday, typeof(LagoVistaCommonStrings))]
        Monday,
        [EnumLabel(Schedule.StrTuesday, LagoVistaCommonStrings.Names.Schedule_Tuesday, typeof(LagoVistaCommonStrings))]
        Tuesday,
        [EnumLabel(Schedule.StrWednesday, LagoVistaCommonStrings.Names.Schedule_Wednesday, typeof(LagoVistaCommonStrings))]
        Wednesday,
        [EnumLabel(Schedule.StrThursday, LagoVistaCommonStrings.Names.Schedule_Thursday, typeof(LagoVistaCommonStrings))]
        Thursday,
        [EnumLabel(Schedule.StrFriday, LagoVistaCommonStrings.Names.Schedule_Friday, typeof(LagoVistaCommonStrings))]
        Friday,
        [EnumLabel(Schedule.StrSaturday, LagoVistaCommonStrings.Names.Schedule_Saturday, typeof(LagoVistaCommonStrings))]
        Saturday,
    }

    [EntityDescription(AuthDomain.AuthenticationDomain, LagoVistaCommonStrings.Names.Schedule_Title, LagoVistaCommonStrings.Names.Schedule_Description, LagoVistaCommonStrings.Names.Schedule_Description,
            EntityDescriptionAttribute.EntityTypes.Dto, typeof(AuthenticationResources), Icon: "icon-ae-calendar")]
    public class Schedule : IFormDescriptor, IFormConditionalFields
    {
        public Schedule()
        {
            Id = Guid.NewGuid().ToId();
            StartsOn = DateTime.UtcNow.ToDateOnly();
        }

        public const string StrSunday = "sunday";
        public const string StrMonday = "monday";
        public const string StrTuesday = "tuesday";
        public const string StrWednesday = "wednesday";
        public const string StrThursday = "thursday";
        public const string StrFriday = "friday";
        public const string StrSaturday = "saturday";


        public const string FirstWeek = "firstweek";
        public const string SecondWeek = "secondweek";
        public const string ThirdWeek = "thirdweek";
        public const string FourthWeek = "fourthweek";

        public const string Daily = "daily";
        public const string Weekly = "weekly";
        public const string Biweekly = "biweekly";
        public const string Monthly = "monthly";
        public const string FirstDayOfMonth = "firstdayofmonth";
        public const string LastDayOfMonth = "lastdayofmonth";

        public string Id { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Name, FieldType: FieldTypes.Text, IsRequired: true, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Name { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Key, FieldType: FieldTypes.Key, IsRequired: true, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Key { get; set; }




        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Description, FieldType: FieldTypes.MultiLineText, IsRequired: false, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Description { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Active, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Active { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_StartsOn, FieldType: FieldTypes.Date, ResourceType: typeof(LagoVistaCommonStrings))]
        public string StartsOn { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_EndsOn, FieldType: FieldTypes.Date, ResourceType: typeof(LagoVistaCommonStrings))]
        public string EndsOn { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_StartTime, FieldType: FieldTypes.Time, ResourceType: typeof(LagoVistaCommonStrings))]
        public string StartTime { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_ScheduleType, EnumType: typeof(ScheduleTypes), FieldType: FieldTypes.Picker, WaterMark: LagoVistaCommonStrings.Names.Schedule_ScheduleType_Select, ResourceType: typeof(LagoVistaCommonStrings))]
        public EntityHeader<ScheduleTypes> ScheduleType { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Sunday, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Sunday { get; set; }
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Monday, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Monday { get; set; }
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Tuesday, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Tuesday { get; set; }
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Wednesday, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Wednesday { get; set; }
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Thursday, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Thursday { get; set; }
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Friday, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Friday { get; set; }
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Saturday, FieldType: FieldTypes.CheckBox, ResourceType: typeof(LagoVistaCommonStrings))]
        public bool Saturday { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_Week, FieldType: FieldTypes.Picker, EnumType: typeof(WeekNumbers), WaterMark: LagoVistaCommonStrings.Names.Schedule_SelectWeek, ResourceType: typeof(LagoVistaCommonStrings))]
        public EntityHeader<WeekNumbers> WeekNumber { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Schedule_DayOfWeek, FieldType: FieldTypes.Picker, EnumType: typeof(WeekDays), WaterMark: LagoVistaCommonStrings.Names.Schedule_SelectWeek, ResourceType: typeof(LagoVistaCommonStrings))]
        public EntityHeader<WeekDays> DayOfWeek { get; set; }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(WeekNumber), nameof(DayOfWeek) },
                Conditionals = new List<FormConditional>()
                {
                     new FormConditional()
                     {
                         Field = nameof(ScheduleType),
                         Value = Schedule.Monthly,
                         Values = new List<string>() {nameof(WeekNumber),nameof(DayOfWeek)},
                     },
                     new FormConditional()
                     {
                         Field = nameof(ScheduleType),
                         Value = Schedule.Weekly,
                         Values = new List<string>() {},
                     },
                     new FormConditional()
                     {
                         Field = nameof(ScheduleType),
                         Value = Schedule.Biweekly,
                         Values = new List<string>() {nameof(DayOfWeek)},
                     },
                }
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Active),
                nameof(StartsOn),
                nameof(EndsOn),
                nameof(StartTime),
                nameof(ScheduleType),
                nameof(WeekNumber),
                nameof(DayOfWeek),
            };
        }


    }
}
