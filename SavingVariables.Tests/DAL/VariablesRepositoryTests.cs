﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SavingVariables.DAL;
using Moq;
using System.Data.Entity;
using SavingVariables.Models;
using System.Collections.Generic;
using System.Linq;

namespace SavingVariables.Tests.DAL
{
    [TestClass]
    public class VariablesRepositoryTests
    {
        Mock<VariablesContext> mock_context { get; set; }
        Mock<DbSet<Variables>> mock_variables_table { get; set; }
        List<Variables> variables_list { get; set; } // Fake

        public void ConnectMocksToDatastore()
        {
            var queryable_list = variables_list.AsQueryable();

            // Lie to LINQ make it think that our new Queryable List is a Database table.
            mock_variables_table.As<IQueryable<Variables>>().Setup(m => m.Provider).Returns(queryable_list.Provider);
            mock_variables_table.As<IQueryable<Variables>>().Setup(m => m.Expression).Returns(queryable_list.Expression);
            mock_variables_table.As<IQueryable<Variables>>().Setup(m => m.ElementType).Returns(queryable_list.ElementType);
            mock_variables_table.As<IQueryable<Variables>>().Setup(m => m.GetEnumerator()).Returns(() => queryable_list.GetEnumerator());

            // Have our Variable property return our Queryable List AKA Fake database table.
            mock_context.Setup(c => c.Variable).Returns(mock_variables_table.Object);


            mock_variables_table.Setup(t => t.Add(It.IsAny<Variables>())).Callback((Variables a) => variables_list.Add(a));
        }

        [TestInitialize]
        public void Initialize()
        {
            // Create Mock BlogContext
            mock_context = new Mock<VariablesContext>();
            mock_variables_table = new Mock<DbSet<Variables>>();
            variables_list = new List<Variables>(); // Fake
        }

        [TestMethod]
        public void RepoEnsureCanCreateInstance()
        {
            VariablesRepository repo = new VariablesRepository();
            Assert.IsNotNull(repo);
        }

        [TestMethod]
        public void RepoEnsureRepoHasContext()
        {
            VariablesRepository repo = new VariablesRepository();

            VariablesContext actual_context = repo.Context;

            Assert.IsInstanceOfType(actual_context, typeof(VariablesContext));
        }

    }
}