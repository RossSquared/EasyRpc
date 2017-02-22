﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EasyRpc.AspNetCore.Middleware;
using Microsoft.AspNetCore.Http;

namespace EasyRpc.AspNetCore
{
    /// <summary>
    /// Configure api 
    /// </summary>
    public interface IApiConfiguration
    {
        /// <summary>
        /// Set default Authorize
        /// </summary>
        /// <param name="role"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        IApiConfiguration Authorize(string role = null, string policy = null);

        /// <summary>
        /// Apply authorize to types
        /// </summary>
        /// <param name="authorizations"></param>
        /// <returns></returns>
        IApiConfiguration Authorize(Func<Type, IEnumerable<IMethodAuthorization>> authorizations);

        /// <summary>
        /// Clear authorize flags
        /// </summary>
        /// <returns></returns>
        IApiConfiguration ClearAuthorize();

        /// <summary>
        /// Apply prefix 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        IApiConfiguration Prefix(string prefix);

        /// <summary>
        /// List of 
        /// </summary>
        /// <param name="prefixFunc"></param>
        /// <returns></returns>
        IApiConfiguration Prefix(Func<Type, IEnumerable<string>> prefixFunc);
        
        /// <summary>
        /// Clear prefixes
        /// </summary>
        /// <returns></returns>
        IApiConfiguration ClearPrefixes();

        /// <summary>
        /// Expose type for RPC
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IExposureConfiguration Expose(Type type);

        /// <summary>
        /// Expose type for RPC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IExposureConfiguration<T> Expose<T>();

        /// <summary>
        /// Expose a set of types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        ITypeSetExposureConfiguration Expose(IEnumerable<Type> types);

        /// <summary>
        /// Apply call filter
        /// </summary>
        /// <returns></returns>
        IApiConfiguration Filter<T>() where T : ICallFilter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterFunc"></param>
        /// <returns></returns>
        IApiConfiguration Filter(Func<Type, Func<HttpContext, IEnumerable<ICallFilter>>> filterFunc);

        /// <summary>
        /// Naming conventions for api
        /// </summary>
        NamingConventions NamingConventions { get; }

        /// <summary>
        /// Add method filter
        /// </summary>
        /// <param name="methodFilter"></param>
        /// <returns></returns>
        IApiConfiguration MethodFilter(Func<MethodInfo, bool> methodFilter);

        /// <summary>
        /// Add exposures to 
        /// </summary>
        /// <param name="provider"></param>
        void AddExposers(IExposedMethodInformationProvider provider);
    }
}
