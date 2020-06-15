/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO UserInfo(Id, FirstName, LastName, Currency) 
VALUES ('32952B48-7D36-48C4-9413-F9E70B002C16', 'Anthony', 'Stark ', 'USD')

INSERT INTO UserInfo(Id, FirstName, LastName, Currency) 
VALUES ('C95F9FEB-31A4-46AD-B3AC-6FEC5804863F', 'Natasha', 'Romanova', 'RUB')
