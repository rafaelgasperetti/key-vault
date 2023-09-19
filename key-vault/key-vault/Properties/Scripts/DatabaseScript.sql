CREATE TABLE IF NOT EXISTS Account
(
	AccountId INT NOT NULL AUTO_INCREMENT,
	Name VARCHAR(40) NOT NULL,
	TenantId VARCHAR(36) NOT NULL,
	ClientId VARCHAR(36) NOT NULL,
	ClientSecret VARCHAR(512) NOT NULL,
	CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	DeletedAt DATETIME,
	PRIMARY KEY(AccountId),
	UNIQUE KEY UK_Account_Name(Name),
	UNIQUE KEY UK_Account_TenantId(TenantId),
	UNIQUE KEY UK_Account_ClientId(ClientId),
	UNIQUE KEY UK_Account_ClientSecret(ClientSecret)
);

CREATE TABLE IF NOT EXISTS SecretKey
(
	SecretKeyId INT NOT NULL AUTO_INCREMENT,
	AccountId INT NOT NULL,
	Name VARCHAR(64) NOT NULL,
	Description TEXT,
	Version INT NOT NULL,
	Value TEXT,
	CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	DeletedAt DATETIME,
	PRIMARY KEY(SecretKeyId),
	CONSTRAINT FK_SecretKey_Account FOREIGN KEY(AccountId) REFERENCES Account(AccountId),
	UNIQUE KEY UK_SecretKey_AccountId_Name_Version(AccountId, Name, Version),
	CONSTRAINT CK_SecretKey_Version CHECK(Version > 0)
);