INSERT INTO tennis_schema.users ("Role", "Username", "Password", "FirstName", "LastName", "Email", "PhoneNumber")
VALUES
(0, 'player1', '$2a$11$zAMfUNOfGa87cxBnueQB3.8IopdUZWClumu9uakPy4zkKK3J.MyOS', 'John', 'Doe', 'lupp.laurentiu@gmail.com', '1234567890'),
(0, 'player2', '$2a$11$zAMfUNOfGa87cxBnueQB3.8IopdUZWClumu9uakPy4zkKK3J.MyOS', 'Jane', 'Smith', 'jane.smith@example.com', '1234567891'),
(0, 'player3', '$2a$11$zAMfUNOfGa87cxBnueQB3.8IopdUZWClumu9uakPy4zkKK3J.MyOS', 'Jim', 'Beam', 'jim.beam@example.com', '1234567892'),
(1, 'referee1', '$2a$11$zAMfUNOfGa87cxBnueQB3.8IopdUZWClumu9uakPy4zkKK3J.MyOS', 'Jack', 'Daniels', 'jack.daniels@example.com', '1234567893'),
(1, 'referee2', '$2a$11$zAMfUNOfGa87cxBnueQB3.8IopdUZWClumu9uakPy4zkKK3J.MyOS', 'Jill', 'Hill', 'jill.hill@example.com', '1234567894'),
(2, 'admin', '$2a$11$zAMfUNOfGa87cxBnueQB3.8IopdUZWClumu9uakPy4zkKK3J.MyOS', 'Admin', 'User', 'admin.user@example.com', '1234567895');

INSERT INTO tennis_schema.tournaments ("Name", "StartDate", "EndDate")
VALUES
('Spring Slam', '2024-03-01', '2024-03-10'),
('Summer Open', '2024-06-01', '2024-06-10'),
('Autumn Classic', '2024-09-01', '2024-09-10'),
('Winter Cup', '2024-12-01', '2024-12-10'),
('Grass Masters', '2024-07-01', '2024-07-10'),
('Hardcourt Pro Series', '2024-08-01', '2024-08-10');

INSERT INTO tennis_schema.tournament_participants ("TournamentId", "UserId")
VALUES
(1, 1),
(1, 2),
(1, 3),
(2, 1),
(2, 2),
(2, 3),
(3, 2),
(3, 3),
(4, 1),
(4, 3),
(5, 3);

INSERT INTO tennis_schema.matches ("Date", "Score", "TournamentId", "PlayerOneId", "PlayerTwoId", "RefereeId")
VALUES
('2024-03-01', '6-3', 1, 1, 2, 5),
('2024-03-02', '7-5', 1, 2, 3, 4),
('2024-06-01', '6-0', 2, 3, 1, 5),
('2024-06-02', '6-4', 2, 1, 2, 4),
('2024-09-01', '7-6', 3, 2, 3, 5),
('2024-12-01', '6-2', 4, 3, 1, 4);
