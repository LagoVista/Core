using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.AutoMapper.TestModels;
using LagoVista.Core.Tests.Mapping;
using LagoVista.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper
{
    public class MappingVerifiers
    {
        [Test]
        public async Task Map_Core_To_DbModel()
        {
            try
            {
                MappingVerifier.Verify<CoreEntity, DbModelBase>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }

        [Test]
        public async Task Map_Relational_To_DbModel()
        {
            try
            {
                MappingVerifier.Verify<RelationalEntityBase, DbModelBase>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }



        [Test]
        public async Task Map_Map_DbModel_To_DbCoreEntity()
        {
            try
            {
                MappingVerifier.Verify<DbModelBase, CoreEntity>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }

        [Test]
        public async Task Map_DbModel_To_DbRelationBase()
        {
            try
            {
                MappingVerifier.Verify<DbModelBase, RelationalEntityBase>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }




        [Test]
        public void TestEHMapping()
        {
            try
            {
                MappingVerifier.Verify<EntityHeaderPrimary, EntityHeaderDTO>(true);
                MappingVerifier.Verify<EntityHeaderDTO, EntityHeaderPrimary>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }


        [Test]
        public async Task MappingForChildDTO_ToParentDTOProp()
        {
            try
            {
                MappingVerifier.Verify<SimpleWithEH, SimpleWithEHDto>(true);
                MappingVerifier.Verify<SimpleWithEHDto, SimpleWithEH>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }

        [Test]
        public async Task Mapping_NullableGuidString36_NullableGuid()
        {
            try
            {
                MappingVerifier.Verify<SourceWithNullableGuidString36, TargetWithNullableGuid>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }


        [Test]
        public async Task Mapping_NullableGuid_NullableGuidString36()
        {
            try
            {
                MappingVerifier.Verify<TargetWithNullableGuid, SourceWithNullableGuidString36>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }

        [Test]
        public async Task TestModels()
        {
            try
            {
                MappingVerifier.Verify<DateMapping, DateMappingDTO>(true);
                MappingVerifier.Verify<DateMappingDTO, DateMapping>(true);

                MappingVerifier.Verify<SimpleManual, SimpleManualDto>(true);
                MappingVerifier.Verify<SimpleManualDto, SimpleManual>(true);

                MappingVerifier.Verify<ChildIdMappingSource, ChildIdMappingTarget>(true);
                MappingVerifier.Verify<RelationalEntityBase, DbModelBase>(true);
                MappingVerifier.Verify<Account, AccountDto>(true);
                MappingVerifier.Verify<PlainEntityHeaderSource, PlainEntityHeaderDestination>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }

    }
}
