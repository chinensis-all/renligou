using Renligou.Core.Shared.Exceptions;

namespace Renligou.Core.Domain.AuthorizationContext.Exception
{
    public class CannotDestroyAdminRoleException : DomainException
    {
        public CannotDestroyAdminRoleException()
        : base("不能删除超级管理员角色")
        {
        }

        public override string ErrorCode => "Role.Administrator.CannotDelete";
    }
}
