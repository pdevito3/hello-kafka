namespace RecipeManagement.Domain.RolePermissions;

using Dtos;
using DomainEvents;
using Roles;
using Domain;
using SharedKernel.Exceptions;

public class RolePermission : BaseEntity
{
    public virtual Role Role { get; private set; }
    public virtual string Permission { get; private set; }


    public static RolePermission Create(RolePermissionForCreationDto rolePermissionForCreationDto)
    {
        ValidationException.Must(BeAnExistingPermission(rolePermissionForCreationDto.Permission), 
            "Please use a valid permission.");

        var newRolePermission = new RolePermission();

        newRolePermission.Role = new Role(rolePermissionForCreationDto.Role);
        newRolePermission.Permission = rolePermissionForCreationDto.Permission;

        newRolePermission.QueueDomainEvent(new RolePermissionCreated(){ RolePermission = newRolePermission });
        
        return newRolePermission;
    }

    public RolePermission Update(RolePermissionForUpdateDto rolePermissionForUpdateDto)
    {
        ValidationException.Must(BeAnExistingPermission(rolePermissionForUpdateDto.Permission), 
            "Please use a valid permission.");

        Role = new Role(rolePermissionForUpdateDto.Role);
        Permission = rolePermissionForUpdateDto.Permission;

        QueueDomainEvent(new RolePermissionUpdated(){ Id = Id });
        return this;
    }
    
    private static bool BeAnExistingPermission(string permission)
    {
        return Permissions.List().Contains(permission, StringComparer.InvariantCultureIgnoreCase);
    }
    
    protected RolePermission() { } // For EF + Mocking
}