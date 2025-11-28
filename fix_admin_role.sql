USE AUTH;
GO

-- Verificar y actualizar el rol del admin
UPDATE Users 
SET Role = 'Admin' 
WHERE Email = 'admin@cuidarmed.com' AND (Role != 'Admin' OR Role IS NULL);
GO

-- Verificar el resultado
SELECT UserId, Email, Role, IsActive 
FROM Users 
WHERE Email = 'admin@cuidarmed.com';
GO




