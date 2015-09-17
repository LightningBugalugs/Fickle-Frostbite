SELECT	sys.server_role_members.role_principal_id, 
		role.name AS RoleName, 
		sys.server_role_members.member_principal_id, 
		member.name AS MemberName
FROM	sys.server_role_members
JOIN sys.server_principals AS role ON sys.server_role_members.role_principal_id = role.principal_id
JOIN sys.server_principals AS member ON sys.server_role_members.member_principal_id = member.principal_id
WHERE	member.name = '{username}'
		AND role.name = 'sysadmin'