using System;

namespace LinguaSpace.Common.Licensing
{
	public interface IRegistration
	{
		String ProductCode
		{
			get;
		}
		String KeyCode
		{
			get;
			set;
		}

		bool IsKeyCodeValid
		{
			get;
		}
	}
}
