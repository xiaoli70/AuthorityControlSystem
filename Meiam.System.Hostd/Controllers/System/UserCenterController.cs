using Meiam.System.Common;
using Meiam.System.Common.Utilities;
using Meiam.System.Hostd.Authorization;
using Meiam.System.Hostd.Extensions;
using Meiam.System.Interfaces;
using Meiam.System.Model;
using Meiam.System.Model.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Meiam.System.Hostd.Controllers.System
{
    /// <summary>
    /// 用户中心
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserCenterController : BaseController
    {
        /// <summary>
        /// 日志管理接口
        /// </summary>
        private readonly ILogger<UserCenterController> _logger;
        /// <summary>
        /// 会话管理接口
        /// </summary>
        private readonly TokenManager _tokenManager;

        /// <summary>
        /// 用户服务接口
        /// </summary>
        private readonly ISysUsersService _usersService;

        /// <summary>
        /// 用户权限接口
        /// </summary>
        private readonly ISysUserRelationService _relationService;

        public UserCenterController(ILogger<UserCenterController> logger, TokenManager tokenManager, ISysUsersService usersService, ISysUserRelationService relationService)
        {
            _logger = logger;
            _tokenManager = tokenManager;
            _usersService = usersService;
            _relationService = relationService;
        }

        /// <summary>
        /// 更新密码（用户自用）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorization]
        public IActionResult UpdatePassword([FromBody] UserCenterUpdatePasswordDto parm)
        {
            if (Convert.ToBoolean(AppSettings.Configuration["AppSettings:Demo"]))
            {
                toResponse(StatusCodeType.Error, "当前为演示模式 , 您无权修改任何数据");
            }

            var userSession = _tokenManager.GetSessionInfo();

            var userInfo = _usersService.GetId(userSession.UserID);

            // 验证旧密码是否正确
            if (!PasswordUtil.ComparePasswords(userInfo.UserID, userInfo.Password, parm.CurrentPassword.Trim()))
            {
                return toResponse(StatusCodeType.Error, "旧密码输入不正确");
            }

            // 更新用户密码
            var response = _usersService.Update(m => m.UserID == userInfo.UserID, m => new Sys_Users()
            {
                Password = PasswordUtil.CreateDbPassword(userInfo.UserID, parm.ConfirmPassword.Trim())
            });

            // 删除登录会话记录
            _tokenManager.RemoveAllSession(userInfo.UserID);

            return toResponse(response);
        }


        /// <summary>
        /// 更新用户信息（用户自用）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorization]
        public IActionResult Update([FromBody] UserCenterUpdateDto parm)
        {
            var userSession = _tokenManager.GetSessionInfo();

            if (Convert.ToBoolean(AppSettings.Configuration["AppSettings:Demo"]))
            {
                toResponse(StatusCodeType.Error, "当前为演示模式 , 您无权修改任何数据");
            }

            #region 更新用户信息
            var response = _usersService.Update(m => m.UserID == userSession.UserID, m => new Sys_Users
            {
                NickName = parm.NickName,
                Email = parm.Email,
                Sex = parm.Sex,
                QQ = parm.QQ,
                Phone = parm.Phone,
                Birthday = parm.Birthday,
                UpdateID = userSession.UserID,
                UpdateName = userSession.UserName,
                UpdateTime = DateTime.Now
            });
            #endregion

            #region 更新登录会话记录

            _tokenManager.RefreshSession(userSession.UserID);

            #endregion

            return toResponse(response);
        }

        /// <summary>
        /// 头像上传接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorization]
        public IActionResult AvatarUpload([FromForm(Name = "file")] IFormFile file)
        {
            try
            {
                if (Convert.ToBoolean(AppSettings.Configuration["AppSettings:Demo"]))
                {
                    return toResponse(StatusCodeType.Error, "当前为演示模式，无法修改数据");
                }

                if (file == null || file.Length == 0)
                {
                    return toResponse(StatusCodeType.Error, "请上传有效的图片文件");
                }

                var fileExtName = Path.GetExtension(file.FileName).ToLower();
                string[] allowedFileExtensions = { ".jpg", ".gif", ".png", ".jpeg" };
                int maxContentLength = 4 * 1024 * 1024; // 4MB

                if (!allowedFileExtensions.Contains(fileExtName, StringComparer.OrdinalIgnoreCase))
                {
                    return toResponse(StatusCodeType.Error, "上传失败，不支持的文件类型");
                }

                if (file.Length > maxContentLength)
                {
                    return toResponse(StatusCodeType.Error, $"上传图片过大，不能超过 {maxContentLength / (1024 * 1024)} MB");
                }
                var path = Path.Combine(Directory.GetCurrentDirectory());
                var fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}{fileExtName}";
                var filePath = Path.Combine(DateTime.Now.ToString("yyyyMMdd"), fileName);
                var avatarDirectory = Path.Combine(path,AppSettings.Configuration["AvatarUpload:AvatarDirectory"], filePath);

                //var directoryPath = Path.GetDirectoryName(avatarDirectory);
                var directoryPath =  Path.GetDirectoryName(avatarDirectory);


                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(avatarDirectory, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var baseUrl = AppSettings.Configuration["AvatarUpload:AvatarUrl"].TrimEnd('/');
                var avatarUrl = $"{baseUrl}/{filePath}".Replace("\\", "/");

                var userSession = _tokenManager.GetSessionInfo();

                _usersService.Update(m => m.UserID == userSession.UserID, m => new Sys_Users
                {
                    AvatarUrl = avatarUrl,
                    UpdateID = userSession.UserID,
                    UpdateName = userSession.UserName,
                    UpdateTime = DateTime.Now
                });
                
                _tokenManager.RefreshSession(userSession.UserID);

                return toResponse(avatarUrl);
            }
            catch (Exception ex)
            {
                return toResponse(StatusCodeType.Error, "上传失败：" + ex.Message);
            }
        }
    }
}
