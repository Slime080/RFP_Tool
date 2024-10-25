INSERT INTO dbo.DropdownList (Id, List, [Description], [Active])
VALUES ((SELECT COUNT(DISTINCT(Id)) FROM dbo.DropdownList) + 1, '', 'CURRENCY', 1)

select * from dbo.DropdownList