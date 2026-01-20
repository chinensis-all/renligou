using Renligou.Core.Shared.Exceptions;

namespace Renligou.Core.Domain.AuthorizationContext.Exception
{
    public class CannotModiifyAdminRoleException : DomainException
    {
        public CannotModiifyAdminRoleException()
        : base("不能修改超级管理员角色")
        {
        }

        public override string ErrorCode => "Role.Administrator.CannotModify";
    }
}
