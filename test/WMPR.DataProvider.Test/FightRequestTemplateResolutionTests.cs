using System;
using NUnit.Framework;
using WMPR.DataProvider.Json;

namespace WMPR.DataProvider.Test
{
	[TestFixture]
	public class FightRequestTemplateResolutionTests
	{
		[Test]
		public void VerifyEmptyTemplate() 
		{
			var context = new FightContextData();

			Assert.Throws<ArgumentNullException>(() => FightRequestTemplateResolver.Resolve(new FightRequestTemplate(string.Empty, string.Empty), context));
			Assert.Throws<ArgumentNullException>(() => FightRequestTemplateResolver.Resolve(null, context));
			Assert.Throws<ArgumentNullException>(() => FightRequestTemplateResolver.Resolve(new FightRequestTemplate(string.Empty, string.Empty), null));
			Assert.Throws<ArgumentNullException>(() => FightRequestTemplateResolver.Resolve(null, null));
		}

		[Test]
		public void VerifyResolution()
		{
			var context = new FightContextData()
			{
				Fight = new Fight()
				{
					end_time = 500,
					start_time = 100
				},
				ReportId = "wasd"
			};

			var template = new FightRequestTemplate(string.Empty, $"test/{RequestWildcard.ReportId.Key}/{RequestWildcard.FightStart.Key}/{RequestWildcard.FightEnd.Key}");
			var resolution = FightRequestTemplateResolver.Resolve(template, context);
			Assert.That(resolution.ResolvedTemplate, Is.EqualTo("test/wasd/100/500"));
			Assert.That(resolution.MappingValues[RequestWildcard.ReportId], Is.EqualTo("wasd"));
			Assert.That(resolution.MappingValues[RequestWildcard.FightStart], Is.EqualTo("100"));
			Assert.That(resolution.MappingValues[RequestWildcard.FightEnd], Is.EqualTo("500"));
		}
	}
}