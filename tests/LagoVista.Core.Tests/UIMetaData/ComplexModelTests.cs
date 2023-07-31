using LagoVista.Core.Models.UIMetaData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.UIMetaData
{

    [TestClass]
    public class ComplexModelTests
    {
        private ComplexModel GetModel()
        {
            var model = new ComplexModel();

            var chld1 = new Child1() { Field1 = "FLD1", Field2 = "FLD2", Field3 = "FLD3" };
            chld1.SubChildList1.Add(new SubChild1() { Field1 = "FLD1", Field2 = "FLD2", Field3 = "FLD3" });
            chld1.SubChildList1.Add(new SubChild1() { Field1 = "FLD4", Field2 = "FLD5", Field3 = "FLD6" });
            model.ChildList1.Add(chld1);

            var chld2 = new Child2() { Field1 = "FLD1", Field2 = "FLD2", Field3 = "FLD3" };
            chld2.SubChildList3.Add(new SubChild3() { Field1 = "FLD1", Field2 = "FLD2", Field3 = "FLD3" });
            chld2.SubChildList3.Add(new SubChild3() { Field1 = "FLD4", Field2 = "FLD5", Field3 = "FLD6" });
            model.ChildList2.Add(chld2);

            return model;
        }

        [TestMethod]
        public void View_On_ChildList_Attribute_Should_Not_Be_Null()
        {

            var model = GetModel();
            var response = DetailResponse<ComplexModel>.Create(model);

            Assert.IsNotNull(response.View[nameof(ComplexModel.ChildList1).ToCamelCase()].View);
            Assert.IsNotNull(response.View[nameof(ComplexModel.ChildList2).ToCamelCase()].View);
        }

        [TestMethod]
        public void View_On_ChildList_Should_Contain_FiledFields()
        {
            var model = GetModel();
            var response = DetailResponse<ComplexModel>.Create(model);
            var fields1 = response.View[nameof(ComplexModel.ChildList1).ToCamelCase()].View;

            Assert.IsTrue(fields1.Keys.Contains(nameof(Child1.Field1).ToCamelCase()));
            Assert.IsTrue(fields1.Keys.Contains(nameof(Child1.Field2).ToCamelCase()));
            Assert.IsTrue(fields1.Keys.Contains(nameof(Child1.Field3).ToCamelCase()));
            Assert.IsTrue(fields1.Keys.Contains(nameof(Child1.SubChildList1).ToCamelCase()));
            Assert.IsTrue(fields1.Keys.Contains(nameof(Child1.SubChildList2).ToCamelCase()));
        }

        [TestMethod]
        public void View_On_ChildLists_ChildList_Should_Contain_FiledFields()
        {
            var model = GetModel();
            var response = DetailResponse<ComplexModel>.Create(model);
            var fields1 = response.View[nameof(ComplexModel.ChildList1).ToCamelCase()].View;
            var fieldsOnChildList = fields1[nameof(Child1.SubChildList1).ToCamelCase()];
            Assert.IsTrue(fieldsOnChildList.View.Keys.Contains(nameof(Child1.Field1).ToCamelCase()));
            Assert.IsTrue(fieldsOnChildList.View.Keys.Contains(nameof(Child1.Field2).ToCamelCase()));
            Assert.IsTrue(fieldsOnChildList.View.Keys.Contains(nameof(Child1.Field3).ToCamelCase()));
        }

        [TestMethod]
        public void View_Should_Contain_ChildLists()
        {
            var model = GetModel();
            var response = DetailResponse<ComplexModel>.Create(model);
            Assert.IsTrue(response.View.Keys.Contains(nameof(ComplexModel.ChildList1).ToCamelCase()));
            Assert.IsTrue(response.View.Keys.Contains(nameof(ComplexModel.ChildList2).ToCamelCase()));
        }


    }

    public static class Extensions
    {
        public static string ToCamelCase(this string value)
        {
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }

    }
}
