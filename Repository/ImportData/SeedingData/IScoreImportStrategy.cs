using System.IO;

namespace Repository.ImportData.SeedingData
{
	public interface IScoreImportStrategy
	{
		void SeedToContext(Stream stream,SeedingContext seedingContext, int year);
	}
}
