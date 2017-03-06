using System;

namespace WMPR.Client.Caliburn
{
	public interface ISupportEditTracking
	{
		bool IsModified { get; }
		event EventHandler<bool> IsModifiedChanged;
		void ClearEditState();
	}
}