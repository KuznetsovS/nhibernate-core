﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.Stateless
{
	using System.Threading.Tasks;
	[TestFixture]
	public class StatelessWithRelationsFixtureAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Stateless.Naturalness.hbm.xml" }; }
		}

		[Test]
		public async Task ShouldWorkLoadingComplexEntitiesAsync()
		{
			const string crocodileFather = "Crocodile father";
			const string crocodileMother = "Crocodile mother";

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var rf = new Reptile { Description = crocodileFather };
				var rm = new Reptile { Description = crocodileMother };
				var rc1 = new Reptile { Description = "Crocodile" };
				var rc2 = new Reptile { Description = "Crocodile" };
				var rfamily = new Family<Reptile>
				{
					Father = rf,
					Mother = rm,
					Childs = new HashSet<Reptile> { rc1, rc2 }
				};
				await (s.SaveAsync("ReptilesFamily", rfamily));
				await (tx.CommitAsync());
			}

			const string humanFather = "Fred";
			const string humanMother = "Wilma";
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var hf = new Human { Description = "Flinstone", Name = humanFather };
				var hm = new Human { Description = "Flinstone", Name = humanMother };
				var hc1 = new Human { Description = "Flinstone", Name = "Pebbles" };
				var hfamily = new Family<Human>
				{
					Father = hf,
					Mother = hm,
					Childs = new HashSet<Human> { hc1 }
				};
				await (s.SaveAsync("HumanFamily", hfamily));
				await (tx.CommitAsync());
			}

			using (IStatelessSession s = Sfi.OpenStatelessSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				IList<Family<Human>> hf = await (s.CreateQuery("from HumanFamily").ListAsync<Family<Human>>());
				Assert.That(hf.Count, Is.EqualTo(1));
				Assert.That(hf[0].Father.Name, Is.EqualTo(humanFather));
				Assert.That(hf[0].Mother.Name, Is.EqualTo(humanMother));
				Assert.That(NHibernateUtil.IsInitialized(hf[0].Childs), Is.True, "No lazy collection should be initialized");
				//Assert.That(hf[0].Childs, Is.Null, "Collections should be ignored by stateless session.");

				IList<Family<Reptile>> rf = await (s.CreateQuery("from ReptilesFamily").ListAsync<Family<Reptile>>());
				Assert.That(rf.Count, Is.EqualTo(1));
				Assert.That(rf[0].Father.Description, Is.EqualTo(crocodileFather));
				Assert.That(rf[0].Mother.Description, Is.EqualTo(crocodileMother));
				Assert.That(NHibernateUtil.IsInitialized(hf[0].Childs), Is.True, "No lazy collection should be initialized");
				//Assert.That(rf[0].Childs, Is.Null, "Collections should be ignored by stateless session.");

				await (tx.CommitAsync());
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				await (s.DeleteAsync("from HumanFamily"));
				await (s.DeleteAsync("from ReptilesFamily"));
				await (tx.CommitAsync());
			}
		}

	}
}