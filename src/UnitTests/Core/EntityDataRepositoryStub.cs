using System;
using System.Collections.Generic;
using Dimensional.TinyReturns.Core.DataRepositories;

namespace Dimensional.TinyReturns.UnitTests.Core
{
    public class EntityDataRepositoryStub : IEntityDataRepository
    {
        private readonly EntityDtoCollectionForTest _entityDtoCollectionForTest;

        public EntityDataRepositoryStub()
        {
            _entityDtoCollectionForTest = new EntityDtoCollectionForTest();
        }

        public void SetupGetAllEntities(
            Action<EntityDtoCollectionForTest> a)
        {
            a(_entityDtoCollectionForTest);
        }
        
        public EntityDto[] GetAllEntities()
        {
            return _entityDtoCollectionForTest.GetEntities();
        }

        public class EntityDtoCollectionForTest
        {
            private readonly List<EntityDto> _entities;

            public EntityDtoCollectionForTest()
            {
                _entities = new List<EntityDto>();
            }

            public EntityDtoCollectionForTest AddPortfolio(
                int number,
                string name)
            {
                _entities.Add(EntityDto.CreateForPortfolio(number, name));

                return this;
            }

            public EntityDtoCollectionForTest AddPortfolio(
                EntityDto entity)
            {
                _entities.Add(entity);

                return this;
            }

            public EntityDto[] GetEntities()
            {
                return _entities.ToArray();
            }
        }
    }
}