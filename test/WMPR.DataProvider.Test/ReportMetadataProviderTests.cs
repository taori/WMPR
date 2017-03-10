using System;
using NUnit.Framework;
using System.Threading.Tasks;

namespace WMPR.DataProvider.Test
{
	[TestFixture]
	public class ReportMetadataProviderTests
	{
		[Test]
		public async Task VerifyRegex()
		{
			var provider = new ReportMetaDataProvider("qQR1mBMT72ztxpkA");
			var result = await provider.GetMetadataAsync();

			Assert.That(result.Reporter, Is.EqualTo("Soulfury"));
			Assert.That(result.Name, Is.EqualTo("Trillax Mythic"));
			Assert.That(result.Date, Is.EqualTo(new DateTime(2017, 1, 11, 21, 9, 32).AddMilliseconds(547)));
		}
	}
}
