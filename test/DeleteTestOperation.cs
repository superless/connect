using System;
using System.Linq;
using trifenix.connect.model;
using trifenix.connect.util;
using Xunit;

namespace trifenix.connect.test.model
{
    public class DeleteTestOperation
    {
        [Fact]
        public void DeleteEntity()
        {
            //asing
            var test = new DeleteItem[]
            {
                new DeleteItem {
                    DocumentType = typeof(BarrackTest).Name,
                    Property = "IdVariety"
                }
            };

            //action
            var delete = Mdm.Reflection.Entities.GetDeleteItem<VarietyTest>();

            //assert
            Assert.True(delete.First().Property.Equals("IdVariety"));
        }
    }
}
