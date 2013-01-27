// dllmain.h : Declaration of module class.

class CLSRAPIModule : public CAtlDllModuleT< CLSRAPIModule >
{
public :
	DECLARE_LIBID(LIBID_LSRAPILib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_LSRAPI, "{0BC6BFA2-1DCA-4E3B-BC1F-18DB8DFE8395}")
};

extern class CLSRAPIModule _AtlModule;
