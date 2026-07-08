select * from Users;

SET IDENTITY_INSERT [Users] ON;

-- Delete existing user 2 if present (optional, only if data is wrong)
-- DELETE FROM Users WHERE UserId = 2;

-- Insert user 2
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = 2)
BEGIN
    INSERT INTO [Users] (UserId, Email, FirstName, LastName, Password)
    VALUES (2, N'jacobtomesh2@gmail.com', N'Jake', N'Tomesh', N'sesame');
END

SET IDENTITY_INSERT [Users] OFF;