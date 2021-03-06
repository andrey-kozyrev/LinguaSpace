// RAPIUtils.h : Declaration of the CRAPIUtils

#pragma once
#include "resource.h"       // main symbols
#include "LSRAPI_i.h"
#include "rapi2.h"

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

class ATL_NO_VTABLE CRAPIUtils :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CRAPIUtils, &CLSID_RAPIUtils>,
	public ISupportErrorInfoImpl<&IID_IRAPIUtils>,
	public IDispatchImpl<IRAPIUtils, &IID_IRAPIUtils, &LIBID_LSRAPILib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CRAPIUtils()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_RAPIUTILS)


BEGIN_COM_MAP(CRAPIUtils)
	COM_INTERFACE_ENTRY(IRAPIUtils)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

	CComPtr<IRAPISession> m_spSession;

public:
	STDMETHOD(GetConnectionInfo)(ConnectionInfo* pConnectionInfo);
};

OBJECT_ENTRY_AUTO(__uuidof(RAPIUtils), CRAPIUtils)
