﻿// auto-generated by x2clr xpiler

using System;
using System.Collections.Generic;
using System.Text;

using x2;

namespace Events.Tests
{
    /// <summary>
    /// id : 1 ~ 1000
    /// </summary>
    public class SampleEvent1 : Event
    {
        protected static new readonly Tag tag;

        public static new int TypeId { get { return tag.TypeId; } }

        private string name_;

        public string Name
        {
            get { return name_; }
            set
            {
                fingerprint.Touch(tag.Offset + 0);
                name_ = value;
            }
        }

        static SampleEvent1()
        {
            tag = new Tag(Event.tag, typeof(SampleEvent1), 1,
                    1);
        }

        public new static SampleEvent1 New()
        {
            return new SampleEvent1();
        }

        public SampleEvent1()
            : base(tag.NumProps)
        {
            Initialize();
        }

        protected SampleEvent1(int length)
            : base(length + tag.NumProps)
        {
            Initialize();
        }

        protected override bool EqualsTo(Cell other)
        {
            if (!base.EqualsTo(other))
            {
                return false;
            }
            SampleEvent1 o = (SampleEvent1)other;
            if (name_ != o.name_)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode(Fingerprint fingerprint)
        {
            var hash = new Hash(base.GetHashCode(fingerprint));
            if (fingerprint.Length <= tag.Offset)
            {
                return hash.Code;
            }
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                hash.Update(tag.Offset + 0);
                hash.Update(name_);
            }
            return hash.Code;
        }

        public override int GetTypeId()
        {
            return tag.TypeId;
        }

        public override Cell.Tag GetTypeTag() 
        {
            return tag;
        }

        public override Func<Event> GetFactoryMethod()
        {
            return SampleEvent1.New;
        }

        protected override bool IsEquivalent(Cell other, Fingerprint fingerprint)
        {
            if (!base.IsEquivalent(other, fingerprint))
            {
                return false;
            }
            SampleEvent1 o = (SampleEvent1)other;
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                if (name_ != o.name_)
                {
                    return false;
                }
            }
            return true;
        }

        public override void Deserialize(Deserializer deserializer)
        {
            base.Deserialize(deserializer);
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                deserializer.Read(out name_);
            }
        }

        public override void Deserialize(VerboseDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            deserializer.Read("Name", out name_);
        }

        public override int GetLength(Type targetType, ref bool flag)
        {
            int length = base.GetLength(targetType, ref flag);
            if (!flag) { return length; }
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                length += Serializer.GetLength(name_);
            }
            if (targetType != null && targetType == typeof(SampleEvent1))
            {
                flag = false;
            }
            return length;
        }

        public override void Serialize(Serializer serializer,
            Type targetType, ref bool flag)
        {
            base.Serialize(serializer, targetType, ref flag);
            if (!flag) { return; }
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                serializer.Write(name_);
            }
            if (targetType != null && targetType == typeof(SampleEvent1))
            {
                flag = false;
            }
        }

        public override void Serialize(VerboseSerializer serializer,
            Type targetType, ref bool flag)
        {
            base.Serialize(serializer, targetType, ref flag);
            if (!flag) { return; }
            serializer.Write("Name", name_);
            if (targetType != null && targetType == typeof(SampleEvent1))
            {
                flag = false;
            }
        }

        protected override void Describe(StringBuilder stringBuilder)
        {
            base.Describe(stringBuilder);
            stringBuilder.AppendFormat(" Name=\"{0}\"", name_.Replace("\"", "\\\""));
        }

        private void Initialize()
        {
            name_ = "";
        }
    }

    public class SampleEvent2 : SampleEvent1
    {
        protected static new readonly Tag tag;

        public static new int TypeId { get { return tag.TypeId; } }

        private string result_;

        public string Result
        {
            get { return result_; }
            set
            {
                fingerprint.Touch(tag.Offset + 0);
                result_ = value;
            }
        }

        static SampleEvent2()
        {
            tag = new Tag(SampleEvent1.tag, typeof(SampleEvent2), 1,
                    2);
        }

        public new static SampleEvent2 New()
        {
            return new SampleEvent2();
        }

        public SampleEvent2()
            : base(tag.NumProps)
        {
            Initialize();
        }

        protected SampleEvent2(int length)
            : base(length + tag.NumProps)
        {
            Initialize();
        }

        protected override bool EqualsTo(Cell other)
        {
            if (!base.EqualsTo(other))
            {
                return false;
            }
            SampleEvent2 o = (SampleEvent2)other;
            if (result_ != o.result_)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode(Fingerprint fingerprint)
        {
            var hash = new Hash(base.GetHashCode(fingerprint));
            if (fingerprint.Length <= tag.Offset)
            {
                return hash.Code;
            }
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                hash.Update(tag.Offset + 0);
                hash.Update(result_);
            }
            return hash.Code;
        }

        public override int GetTypeId()
        {
            return tag.TypeId;
        }

        public override Cell.Tag GetTypeTag() 
        {
            return tag;
        }

        public override Func<Event> GetFactoryMethod()
        {
            return SampleEvent2.New;
        }

        protected override bool IsEquivalent(Cell other, Fingerprint fingerprint)
        {
            if (!base.IsEquivalent(other, fingerprint))
            {
                return false;
            }
            SampleEvent2 o = (SampleEvent2)other;
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                if (result_ != o.result_)
                {
                    return false;
                }
            }
            return true;
        }

        public override void Deserialize(Deserializer deserializer)
        {
            base.Deserialize(deserializer);
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                deserializer.Read(out result_);
            }
        }

        public override void Deserialize(VerboseDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            deserializer.Read("Result", out result_);
        }

        public override int GetLength(Type targetType, ref bool flag)
        {
            int length = base.GetLength(targetType, ref flag);
            if (!flag) { return length; }
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                length += Serializer.GetLength(result_);
            }
            if (targetType != null && targetType == typeof(SampleEvent2))
            {
                flag = false;
            }
            return length;
        }

        public override void Serialize(Serializer serializer,
            Type targetType, ref bool flag)
        {
            base.Serialize(serializer, targetType, ref flag);
            if (!flag) { return; }
            var touched = new Capo<bool>(fingerprint, tag.Offset);
            if (touched[0])
            {
                serializer.Write(result_);
            }
            if (targetType != null && targetType == typeof(SampleEvent2))
            {
                flag = false;
            }
        }

        public override void Serialize(VerboseSerializer serializer,
            Type targetType, ref bool flag)
        {
            base.Serialize(serializer, targetType, ref flag);
            if (!flag) { return; }
            serializer.Write("Result", result_);
            if (targetType != null && targetType == typeof(SampleEvent2))
            {
                flag = false;
            }
        }

        protected override void Describe(StringBuilder stringBuilder)
        {
            base.Describe(stringBuilder);
            stringBuilder.AppendFormat(" Result=\"{0}\"", result_.Replace("\"", "\\\""));
        }

        private void Initialize()
        {
            result_ = "";
        }
    }
}
