using System;
using System.IO;
using System.Diagnostics;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.Common.Licensing
{
	public class RegistrationServiceValidator : Validator
	{
		private RegistrationService service;

		public RegistrationServiceValidator(RegistrationService service)
		{
			Debug.Assert(service != null);
			this.service = service;
			Hook(service.Registration);
			Validate();
		}

		protected override void ValidateOverride()
		{
			base.ValidateOverride();
			if (this.IsValid)
			{
				if (!this.service.Registration.IsKeyCodeValid)
				{
					SetStatus(false, CommonStrings.KEY_CODE_IS_NOT_VALID, ValidationMessageType.Warning);
				}
			}
		}

		protected override string ValidationSuccessMessage
		{
			get
			{
                return CommonStrings.KEY_CODE_IS_VALID;
			}
		}
	}

	public class RegistrationService
	{
		static private RegistrationService service = null;

		public static RegistrationService Service
		{
			get
			{
				lock (typeof(RegistrationService))
				{
					if (service == null)
					{
						service = new RegistrationService();
					}
				}
				return service;
			}
		}

		private IRegistration reg = null;

		private ITrial trial = null;

		private RegistrationService()
		{
		}

		public void Initialize()
		{
			lock (this)
			{
				if (this.reg == null && this.trial == null)
				{
					this.reg = new Registration();
					this.trial = new TrialCounter();
				}
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public IRegistration Registration
		{
			get
			{
				return this.reg;
			}
		}

		public ITrial Trial
		{
			get
			{
				return this.trial;
			}
		}
	}
}
