

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0500 */
/* at Tue Jan 13 00:27:24 2009
 */
/* Compiler settings for .\LSRAPI.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __LSRAPI_i_h__
#define __LSRAPI_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IRAPIUtils_FWD_DEFINED__
#define __IRAPIUtils_FWD_DEFINED__
typedef interface IRAPIUtils IRAPIUtils;
#endif 	/* __IRAPIUtils_FWD_DEFINED__ */


#ifndef __RAPIUtils_FWD_DEFINED__
#define __RAPIUtils_FWD_DEFINED__

#ifdef __cplusplus
typedef class RAPIUtils RAPIUtils;
#else
typedef struct RAPIUtils RAPIUtils;
#endif /* __cplusplus */

#endif 	/* __RAPIUtils_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_LSRAPI_0000_0000 */
/* [local] */ 


enum ConnectionType
    {	USB	= 0,
	InfraRed	= 1,
	Serial	= 2,
	Network	= 3
    } ;
struct SocketAddress
    {
    long ip;
    long port;
    } ;
struct ConnectionInfo
    {
    long connectionType;
    struct SocketAddress device;
    struct SocketAddress desktop;
    } ;


extern RPC_IF_HANDLE __MIDL_itf_LSRAPI_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_LSRAPI_0000_0000_v0_0_s_ifspec;

#ifndef __IRAPIUtils_INTERFACE_DEFINED__
#define __IRAPIUtils_INTERFACE_DEFINED__

/* interface IRAPIUtils */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IRAPIUtils;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E78EBC7D-F974-4A86-8560-28E1CBBCE7C5")
    IRAPIUtils : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetConnectionInfo( 
            /* [retval][out] */ struct ConnectionInfo *pConnectionInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IRAPIUtilsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IRAPIUtils * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IRAPIUtils * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IRAPIUtils * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IRAPIUtils * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IRAPIUtils * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IRAPIUtils * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IRAPIUtils * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetConnectionInfo )( 
            IRAPIUtils * This,
            /* [retval][out] */ struct ConnectionInfo *pConnectionInfo);
        
        END_INTERFACE
    } IRAPIUtilsVtbl;

    interface IRAPIUtils
    {
        CONST_VTBL struct IRAPIUtilsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IRAPIUtils_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IRAPIUtils_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IRAPIUtils_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IRAPIUtils_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IRAPIUtils_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IRAPIUtils_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IRAPIUtils_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IRAPIUtils_GetConnectionInfo(This,pConnectionInfo)	\
    ( (This)->lpVtbl -> GetConnectionInfo(This,pConnectionInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IRAPIUtils_INTERFACE_DEFINED__ */



#ifndef __LSRAPILib_LIBRARY_DEFINED__
#define __LSRAPILib_LIBRARY_DEFINED__

/* library LSRAPILib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_LSRAPILib;

EXTERN_C const CLSID CLSID_RAPIUtils;

#ifdef __cplusplus

class DECLSPEC_UUID("39515898-AF1A-4072-AFF7-F75C6956A8BD")
RAPIUtils;
#endif
#endif /* __LSRAPILib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


