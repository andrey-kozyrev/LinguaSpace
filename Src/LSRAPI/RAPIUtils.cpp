// RAPIUtils.cpp : Implementation of CRAPIUtils

#include "stdafx.h"
#include "RAPIUtils.h"

STDMETHODIMP CRAPIUtils::GetConnectionInfo(ConnectionInfo* pConnectionInfo)
{
	ATLASSERT(pConnectionInfo != NULL);
	memset(pConnectionInfo, 0, sizeof(ConnectionInfo));

	HRESULT hr;

	CComPtr<IRAPIDesktop> spDesktop;
	hr = spDesktop.CoCreateInstance(CLSID_RAPI);
	ATLASSERT(spDesktop != NULL);
	if (FAILED(hr))
		return hr;
	
	CComPtr<IRAPIEnumDevices> spDevices;
	hr = spDesktop->EnumDevices(&spDevices);
	ATLASSERT(spDevices != NULL);
	if (FAILED(hr))
		return hr;

	CComPtr<IRAPIDevice> spDevice;
	hr = spDevices->Next(&spDevice);
	if (FAILED(hr))
		return hr;

	RAPI_CONNECTIONINFO connectionInfo = {0};
	hr = spDevice->GetConnectionInfo(&connectionInfo);
	if (FAILED(hr))
		return hr;

	sockaddr_in* pDesktopSocAddress = reinterpret_cast<sockaddr_in*>(&connectionInfo.hostIpaddr);
	sockaddr_in* pDeviceSocAddress = reinterpret_cast<sockaddr_in*>(&connectionInfo.ipaddr);

	pConnectionInfo->connectionType = connectionInfo.connectionType;
	pConnectionInfo->desktop.ip = pDesktopSocAddress->sin_addr.s_addr;
	pConnectionInfo->desktop.port = pDesktopSocAddress->sin_port;
	pConnectionInfo->device.ip = pDeviceSocAddress->sin_addr.s_addr;
	pConnectionInfo->device.port = pDeviceSocAddress->sin_port;


	hr = spDevice->CreateSession(&m_spSession);
	ATLASSERT(m_spSession != NULL);
	if (FAILED(hr))
		return hr;
	
	return m_spSession->CeRapiInit();


	return S_OK;
}
