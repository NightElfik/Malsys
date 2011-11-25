-- Altairis Web Security Toolkit - database creation script for "Table*Provider" classes
-- Copyright © Michal A. Valasek - Altairis, 2006-2010 | www.altairis.cz | www.rider.cz
-- Licensed under terms of Microsoft Shared Source Permissive License (MS-PL)

-- Table for TableMembershipProvider
CREATE TABLE dbo.Users (
	UserName				nvarchar(100)		NOT NULL,
	PasswordHash			binary(64)			NOT NULL,
	PasswordSalt			binary(128)			NOT NULL,
	Email					nvarchar(max)		NOT NULL,
	Comment					nvarchar(max)		NULL,
	IsApproved				bit					NOT NULL,
	DateCreated				datetime			NOT NULL,
	DateLastLogin			datetime			NULL,
	DateLastActivity		datetime			NULL,
	DateLastPasswordChange	datetime			NOT NULL,
	-- You may add other columns needed by your application, as
	-- long as they are either nullable or have defaults assigned
	CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserName)
)

-- Tables for TableRoleProvider
CREATE TABLE dbo.Roles (
	RoleName				nvarchar(100)		NOT NULL,
	CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleName)
)
CREATE TABLE dbo.RoleMemberships (
	UserName				nvarchar(100)		NOT NULL,
	RoleName				nvarchar(100)		NOT NULL,
	CONSTRAINT PK_RoleMemberships PRIMARY KEY CLUSTERED (UserName, RoleName),
	CONSTRAINT FK_RoleMemberships_Roles FOREIGN KEY (RoleName) REFERENCES dbo.Roles (RoleName) ON UPDATE CASCADE ON DELETE CASCADE,
)

-- When using both these providers together, you may want to add the foreign key
ALTER TABLE dbo.RoleMemberships ADD CONSTRAINT FK_RoleMemberships_Users FOREIGN KEY (UserName) REFERENCES dbo.Users (UserName) ON UPDATE CASCADE ON DELETE CASCADE
