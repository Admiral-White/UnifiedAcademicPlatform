-- Create databases for each service
CREATE DATABASE UAP_Authentication;
CREATE DATABASE UAP_CourseCatalog;
CREATE DATABASE UAP_StudentAcademic;
CREATE DATABASE UAP_Registration;
CREATE DATABASE UAP_Notification;
CREATE DATABASE UAP_Reporting;

-- Create login for application
CREATE LOGIN uap_app WITH PASSWORD = 'YourStrong!Password123';

-- Configure databases for each service
USE UAP_Authentication;
CREATE USER uap_app FOR LOGIN uap_app;
ALTER ROLE db_owner ADD MEMBER uap_app;

USE UAP_CourseCatalog;
CREATE USER uap_app FOR LOGIN uap_app;
ALTER ROLE db_owner ADD MEMBER uap_app;

USE UAP_StudentAcademic;
CREATE USER uap_app FOR LOGIN uap_app;
ALTER ROLE db_owner ADD MEMBER uap_app;

USE UAP_Registration;
CREATE USER uap_app FOR LOGIN uap_app;
ALTER ROLE db_owner ADD MEMBER uap_app;

USE UAP_Notification;
CREATE USER uap_app FOR LOGIN uap_app;
ALTER ROLE db_owner ADD MEMBER uap_app;

USE UAP_Reporting;
CREATE USER uap_app FOR LOGIN uap_app;
ALTER ROLE db_owner ADD MEMBER uap_app;