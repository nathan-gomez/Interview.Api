CREATE TABLE users
(
    id         uniqueidentifier NOT NULL,
    username   varchar(500)     NOT NULL,
    password   varchar(100)     NOT NULL,
    created_at datetime2        NOT NULL,
    PRIMARY KEY (id)
);
CREATE UNIQUE INDEX idx_username ON users (username);

CREATE TABLE sessions
(
    id         uniqueidentifier NOT NULL,
    user_id    uniqueidentifier NOT NULL,
    expiration datetime2        NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (user_id) REFERENCES users (id)
);

CREATE TABLE clients
(
    id           int IDENTITY (1,1) PRIMARY KEY,
    name         varchar(500) NOT NULL,
    document_id  varchar(100) NOT NULL,
    phone_number varchar(200),
    observation varchar(max)
);

insert into users (id,username, password, created_at)
VALUES ('6bcd52f1-8b18-4d82-9acb-3fe3b4125ff6','Admin','$2a$13$EwEKPZ0p.a2v5r1KloDhdet5EQa7587pOZSFXKCewYcRxz9B6Px1.','2024-04-21');