// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5dbefad3d483788bbfce3a1b4a663584d39df312e1bbdde7accc4baa9f9b3ed5
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Compare;
using LagoVista.Core.Models;
using LagoVista.Core.Resources;
using LagoVista.Core.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Compare
{
    [TestClass]
    public class ComparatorTests
    {
        private const int VALUE_1_ORIGINAL = 42;
        private const int NULLABLE_ORIGINAL = 100;

        [TestMethod]
        public void SimpleCompare_TypeMismatch_Exception()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var objA = new Models.ComparatorModelSimple();
                var objC = new Models.ComparatorModelModerate();


                var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");

                Comparator.Compare(objA, objC, user);
            });
        }

        [TestMethod]
        public void SimpleCompare_MissingUser_Exception()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                Models.ComparatorModelModerate objA = null;
                Models.ComparatorModelModerate objB = null;
                EntityHeader user = null;

                Comparator.Compare(objA, objB, user);
            });
        }

        [TestMethod]
        public void SimpleCompare_EmptyUser_Exception()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                Models.ComparatorModelModerate objA = null;
                Models.ComparatorModelModerate objB = null;
                var user = new EntityHeader() { Id = Guid.NewGuid().ToId() };

                Comparator.Compare(objA, objB, user);
            });
        }



        [TestMethod]
        public void SimpleCompare_BothNull_Exception()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                Models.ComparatorModelModerate objA = null;
                Models.ComparatorModelModerate objB = null;

                var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");

                Comparator.Compare(objA, objB, user);
            });
        }

        [TestMethod]
        public void SimpleCompare_FirstNull_Exception()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                Models.ComparatorModelModerate objA = null;
                var objB = new Models.ComparatorModelSimple();

                var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");

                Comparator.Compare(objA, objB, user);
            });
        }

        [TestMethod]
        public void SimpleCompare_SecondNull_Exception()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var objA = new Models.ComparatorModelSimple();
                Models.ComparatorModelModerate objB = null;

                var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");

                Comparator.Compare(objA, objB, user);
            });
        }


        [TestMethod]
        public void SimpleCompare_IDMisMatch_Exception()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var objA = new Models.ComparatorModelSimple();
                var objB = new Models.ComparatorModelModerate();

                objA.Id = Guid.NewGuid().ToId();
                objB.Id = Guid.NewGuid().ToId();

                var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");

                Comparator.Compare(objA, objB, user);
            });
        }

        [TestMethod]
        public void SimpleCompare_PopulatesUserAndDateStamp_Valid()
        {
            var id = Guid.NewGuid().ToId();

            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");

            var result = Comparator.Compare(objA, objB, user);
            Assert.AreEqual(user.Id, result.User.Id);
            Assert.AreEqual(user.Text, result.User.Text);
            Assert.IsTrue(result.DateStamp.SuccessfulJSONDate());
        }

        public ComparatorModelSimple GetSimpleModel(String id)
        {
            return new Models.ComparatorModelSimple()
            {
                Id = id,
                FirstName = "Kevin",
                Value1 = VALUE_1_ORIGINAL,
                NullableValue = NULLABLE_ORIGINAL
            };

        }

        [TestMethod]
        public void SimpleCompare_Match()
        {
            var id = Guid.NewGuid().ToId();

            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsFalse(result.IsDirty);
        }

        [TestMethod]
        public void SimpleCompare_FirstEmpty_SecondNotNull_NoMatch()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objA.FirstName = String.Empty;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(ComparatorResources.EmptyValue, result.Changes.First().OldValue);
            Assert.AreEqual("Kevin", result.Changes.First().NewValue);
            Assert.AreEqual(Resources.Models.ValidationResources.FirstNameLabel, result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_FirstNull_SecondNotNull_NoMatch()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objA.FirstName = null;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(ComparatorResources.EmptyValue, result.Changes.First().OldValue);
            Assert.AreEqual("Kevin", result.Changes.First().NewValue);
            Assert.AreEqual(Resources.Models.ValidationResources.FirstNameLabel, result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_SecondEmpty_SecondNotNull_NoMatch()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objB.FirstName = String.Empty;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(ComparatorResources.EmptyValue, result.Changes.First().NewValue);
            Assert.AreEqual("Kevin", result.Changes.First().OldValue);
            Assert.AreEqual(Resources.Models.ValidationResources.FirstNameLabel, result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_SecondNull_SecondNotNull_NoMatch()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objB.FirstName = null;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(ComparatorResources.EmptyValue, result.Changes.First().NewValue);
            Assert.AreEqual("Kevin", result.Changes.First().OldValue);
            Assert.AreEqual(Resources.Models.ValidationResources.FirstNameLabel, result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_Change_BothValues_NoMatch()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objB.FirstName = "Beth";

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual("Beth", result.Changes.First().NewValue);
            Assert.AreEqual("Kevin", result.Changes.First().OldValue);
            Assert.AreEqual(Resources.Models.ValidationResources.FirstNameLabel, result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_ChangeValue1_First_To_44_Value()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objB.Value1 = 44;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(objB.Value1.ToString(), result.Changes.First().NewValue);
            Assert.AreEqual(VALUE_1_ORIGINAL.ToString(), result.Changes.First().OldValue);
            Assert.AreEqual(nameof(objA.Value1), result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_Change_Nullable_From_NullToValue()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objA.NullableValue = null;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(ComparatorResources.EmptyValue, result.Changes.First().OldValue);
            Assert.AreEqual(NULLABLE_ORIGINAL.ToString(), result.Changes.First().NewValue);            
            Assert.AreEqual(nameof(objA.NullableValue), result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_Change_Nullable_From_ValueToNull()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objB.NullableValue = null;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(NULLABLE_ORIGINAL.ToString(), result.Changes.First().OldValue);
            Assert.AreEqual(ComparatorResources.EmptyValue, result.Changes.First().NewValue);
            
            Assert.AreEqual(nameof(objA.NullableValue), result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }

        [TestMethod]
        public void SimpleCompare_Change_Nullable_From_ValueToDiffentValue()
        {
            var id = Guid.NewGuid().ToId();
            var objA = GetSimpleModel(id);
            var objB = GetSimpleModel(id);
            objB.NullableValue = 150;

            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "SOME_USER");
            var result = Comparator.Compare(objA, objB, user);
            Assert.IsTrue(result.IsDirty);
            Assert.AreEqual(NULLABLE_ORIGINAL.ToString(), result.Changes.First().OldValue);
            Assert.AreEqual(objB.NullableValue.Value.ToString(), result.Changes.First().NewValue);

            Assert.AreEqual(nameof(objA.NullableValue), result.Changes.First().Name);
            Console.WriteLine(result.Changes.First());
        }


    }
}
