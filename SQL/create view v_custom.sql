select a.user_name,b.email,a.password from online_user as a
inner join customer as b 
on a.customer_id = b.customer_id;

CREATE VIEW `v_custom` AS
select a.user_name,b.email,a.password from online_user as a
inner join customer as b 
on a.customer_id = b.customer_id;
