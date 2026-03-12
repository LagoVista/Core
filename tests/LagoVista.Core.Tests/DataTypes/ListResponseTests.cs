using System;
using System.Collections.Generic;
using System.Linq;
using LagoVista.Core.Models.UIMetaData;
using NUnit.Framework;

namespace LagoVista.Core.Tests.Models.UIMetaData
{
    [TestFixture]
    public class ListResponsePagingTests
    {
        private sealed class TestModel
        {
            public string Id { get; set; }
            public DateTime PartitionDate { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
        }

        [Test]
        public void Create_WithListRequest_Should_Set_PageIndex_PageSize_And_HasMoreRecords_False_When_Model_Count_Is_Less_Than_PageSize()
        {
            var request = new ListRequest
            {
                PageIndex = 2,
                PageSize = 5
            };

            var items = CreateModels(3);

            var response = ListResponse<TestModel>.Create(items, request);

            Assert.That(response.PageIndex, Is.EqualTo(2));
            Assert.That(response.PageSize, Is.EqualTo(5));
            Assert.That(response.RecordCount, Is.EqualTo(3));
            Assert.That(response.HasMoreRecords, Is.False);
            Assert.That(response.Model.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Create_WithListRequest_Should_Set_HasMoreRecords_True_When_Model_Count_Equals_PageSize()
        {
            var request = new ListRequest
            {
                PageIndex = 0,
                PageSize = 5
            };

            var items = CreateModels(5);

            var response = ListResponse<TestModel>.Create(items, request);

            Assert.That(response.PageIndex, Is.EqualTo(0));
            Assert.That(response.PageSize, Is.EqualTo(5));
            Assert.That(response.RecordCount, Is.EqualTo(5));
            Assert.That(response.HasMoreRecords, Is.True);
            Assert.That(response.Model.Count(), Is.EqualTo(5));
        }

        [Test]
        public void Create_RequestFirstOverload_Should_Set_GetListUrl_And_HasMoreRecords_Based_On_Model_Count()
        {
            var request = new ListRequest
            {
                PageIndex = 1,
                PageSize = 3,
                Url = "/api/testmodels"
            };

            var items = CreateModels(3);

            var response = ListResponse<TestModel>.Create(request, items);

            Assert.That(response.PageIndex, Is.EqualTo(1));
            Assert.That(response.PageSize, Is.EqualTo(3));
            Assert.That(response.GetListUrl, Is.EqualTo("/api/testmodels"));
            Assert.That(response.RecordCount, Is.EqualTo(3));
            Assert.That(response.HasMoreRecords, Is.True);
            Assert.That(response.Model.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Create_RequestFirstOverload_Should_Set_HasMoreRecords_False_When_Model_Count_Is_Not_PageSize()
        {
            var request = new ListRequest
            {
                PageIndex = 4,
                PageSize = 3,
                Url = "/api/testmodels"
            };

            var items = CreateModels(2);

            var response = ListResponse<TestModel>.Create(request, items);

            Assert.That(response.PageIndex, Is.EqualTo(4));
            Assert.That(response.PageSize, Is.EqualTo(3));
            Assert.That(response.GetListUrl, Is.EqualTo("/api/testmodels"));
            Assert.That(response.RecordCount, Is.EqualTo(2));
            Assert.That(response.HasMoreRecords, Is.False);
            Assert.That(response.Model.Count(), Is.EqualTo(2));
        }




        [Test]
        public void Create_FromOriginalListResponse_Should_Copy_Paging_Fields()
        {
            var source = new ListResponse<TestModel>
            {
                Title = "Test Title",
                Help = "Test Help",
                Columns = new List<ListColumn>(),
                FactoryUrl = "/factory",
                GetListUrl = "/list",
                DeleteUrl = "/delete",
                HelpUrl = "/help",
                GetUrl = "/get",
                PageCount = 9,
                PageIndex = 4,
                PageSize = 25,
                RecordCount = 200,
                NextPartitionKey = "next-pk",
                NextRowKey = "next-rk",
                HasMoreRecords = true
            };

            var items = CreateModels(2);

            var response = source.Create(items);

            Assert.That(response.PageCount, Is.EqualTo(9));
            Assert.That(response.PageIndex, Is.EqualTo(4));
            Assert.That(response.PageSize, Is.EqualTo(25));
            Assert.That(response.RecordCount, Is.EqualTo(200));
            Assert.That(response.NextPartitionKey, Is.EqualTo("next-pk"));
            Assert.That(response.NextRowKey, Is.EqualTo("next-rk"));
            Assert.That(response.HasMoreRecords, Is.True);
            Assert.That(response.Model.Count(), Is.EqualTo(2));
        }

        private static List<TestModel> CreateModels(int count)
        {
            var items = new List<TestModel>();

            for (var idx = 1; idx <= count; idx++)
            {
                items.Add(new TestModel
                {
                    Id = idx.ToString(),
                    PartitionDate = new DateTime(2026, 1, idx, 8, 0, 0, DateTimeKind.Utc),
                    PartitionKey = $"pk-{idx}",
                    RowKey = $"rk-{idx}"
                });
            }

            return items;
        }
    }
}