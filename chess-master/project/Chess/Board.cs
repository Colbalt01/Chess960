using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    public class ChessBoard
    {
        private static int[] pieceWeights = { 1, 3, 4, 5, 7, 20 };

        public piece_t[][] Grid { get; private set; }
        public Dictionary<Player, position_t> Kings { get; private set; }
        public Dictionary<Player, List<position_t>> Pieces { get; private set; }
        public Dictionary<Player, position_t> LastMove { get; private set; }

        public bool c960 = false;

        public ChessBoard()
        {
            // init blank board grid
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                    Grid[i][j] = new piece_t(Piece.NONE, Player.WHITE);
            }

            // init last moves
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t();
            LastMove[Player.WHITE] = new position_t();

            // init king positions
            Kings = new Dictionary<Player, position_t>();

            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());
        }

        public ChessBoard(ChessBoard copy)
        {
            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());

            // init board grid to copy locations
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                {
                    Grid[i][j] = new piece_t(copy.Grid[i][j]);

                    // add piece location to list
                    if (Grid[i][j].piece != Piece.NONE)
                        Pieces[Grid[i][j].player].Add(new position_t(j, i));
                }
            }

            // copy last known move
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t(copy.LastMove[Player.BLACK]);
            LastMove[Player.WHITE] = new position_t(copy.LastMove[Player.WHITE]);

            // copy king locations
            Kings = new Dictionary<Player, position_t>();
            Kings[Player.BLACK] = new position_t(copy.Kings[Player.BLACK]);
            Kings[Player.WHITE] = new position_t(copy.Kings[Player.WHITE]);
        }

        /// <summary>
        /// Calculate and return the boards fitness value.
        /// </summary>
        /// <param name="max">Who's side are we viewing from.</param>
        /// <returns>The board fitness value, what else?</returns>
        public int fitness(Player max)
        {
            int fitness = 0;
            int[] blackPieces = { 0, 0, 0, 0, 0, 0 };
            int[] whitePieces = { 0, 0, 0, 0, 0, 0 };
            int blackMoves = 0;
            int whiteMoves = 0;

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.BLACK])
            {
                blackMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                blackPieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.WHITE])
            {
                whiteMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                whitePieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // if viewing from black side
            if (max == Player.BLACK)
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (blackPieces[i] - whitePieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (blackMoves - whiteMoves));
            }
            else
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (whitePieces[i] - blackPieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (whiteMoves - blackMoves));
            }

            return fitness;
        }

        public void SetInitialPlacement()
        {
            if (c960 == true)
            {
                for (int i = 0; i < 8; i++)
                {
                    SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                    SetPiece(Piece.PAWN, Player.BLACK, i, 6);
                }

                SetPiece(Piece.ROOK, Player.WHITE, 0, 0);
                SetPiece(Piece.ROOK, Player.WHITE, 7, 0);
                SetPiece(Piece.ROOK, Player.BLACK, 0, 7);
                SetPiece(Piece.ROOK, Player.BLACK, 7, 7);

                SetPiece(Piece.KNIGHT, Player.WHITE, 1, 0);
                SetPiece(Piece.KNIGHT, Player.WHITE, 6, 0);
                SetPiece(Piece.KNIGHT, Player.BLACK, 1, 7);
                SetPiece(Piece.KNIGHT, Player.BLACK, 6, 7);

                SetPiece(Piece.BISHOP, Player.WHITE, 2, 0);
                SetPiece(Piece.BISHOP, Player.WHITE, 5, 0);
                SetPiece(Piece.BISHOP, Player.BLACK, 2, 7);
                SetPiece(Piece.BISHOP, Player.BLACK, 5, 7);

                SetPiece(Piece.KING, Player.WHITE, 4, 0);
                SetPiece(Piece.KING, Player.BLACK, 4, 7);
                Kings[Player.WHITE] = new position_t(4, 0);
                Kings[Player.BLACK] = new position_t(4, 7);
                SetPiece(Piece.QUEEN, Player.WHITE, 3, 0);
                SetPiece(Piece.QUEEN, Player.BLACK, 3, 7);
            } else
            {
				for (int i = 0; i < 8; i++)
				{
					SetPiece(Piece.PAWN, Player.WHITE, i, 1);
					SetPiece(Piece.PAWN, Player.BLACK, i, 6);
				}

				//https://stackoverflow.com/questions/11984709/random-number-generator-in-c-sharp
				Random rnd = new Random();
				int n = rnd.Next(960);
				int n2 = n / 4;
				int b1 = n % 4;
                int n3 = n2 / 4;
                int b2 = n2 % 4;
                int n4 = n3 / 6;
                int q = n3 & 6;
                int[] b = new int[8];

                int bk2 = b2 * 2;
                int bk1 = b1 + (b1 + 1);

				switch (b1)
				{
					case 0:
						SetPiece(Piece.BISHOP, Player.WHITE, 1, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 1, 7);
                        b[1] = 1;
						break;
                    case 1:
						SetPiece(Piece.BISHOP, Player.WHITE, 3, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 3, 7);
						b[3] = 1;
						break;
                    case 2:
						SetPiece(Piece.BISHOP, Player.WHITE, 5, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 5, 7);
						b[5] = 1;
						break;
                    case 3:
						SetPiece(Piece.BISHOP, Player.WHITE, 7, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 7, 7);
						b[7] = 1;
						break;
				}
				switch (b2)
				{
					case 0:
						SetPiece(Piece.BISHOP, Player.WHITE, 0, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 0, 7);
						b[0] = 1;
						break;
					case 1:
						SetPiece(Piece.BISHOP, Player.WHITE, 2, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 2, 7);
                        b[2] = 1;
						break;
					case 2:
						SetPiece(Piece.BISHOP, Player.WHITE, 4, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 4, 7);
						b[4] = 1;
						break;
					case 3:
						SetPiece(Piece.BISHOP, Player.WHITE, 6, 0);
						SetPiece(Piece.BISHOP, Player.BLACK, 6, 7);
						b[6] = 1;
						break;
				}

                if (q!=(b2*2) & q != (b1 + (b1 + 1)))
                {
					SetPiece(Piece.QUEEN, Player.WHITE, q, 0);
					SetPiece(Piece.QUEEN, Player.BLACK, q, 7);
					b[q] = 1;
				} else if (q+1 != (b2 * 2) & q+1 != (b1 + (b1 + 1)))
                {
					SetPiece(Piece.QUEEN, Player.WHITE, q+1, 0);
					SetPiece(Piece.QUEEN, Player.BLACK, q+1, 7);
					b[q] = 1;
				}

                int[] knightP = new int[2];

                switch (n4)
                {
                    case 0:
                        knightP[0] = 0;
                        knightP[1] = 1;
                        break;
                    case 1:
                        knightP[0] = 0;
                        knightP[1] = 2;
                        break;
					case 2:
						knightP[0] = 0;
						knightP[1] = 3;
						break;
					case 3:
						knightP[0] = 0;
						knightP[1] = 4;
						break;
					case 4:
						knightP[0] = 1;
						knightP[1] = 2;
						break;

					case 5:
						knightP[0] = 1;
						knightP[1] = 3;
						break;
					case 6:
						knightP[0] = 1;
						knightP[1] = 4;
						break;
					case 7:
						knightP[0] = 2;
						knightP[1] = 3;
						break;
					case 8:
						knightP[0] = 2;
						knightP[1] = 4;
						break;
					case 9:
						knightP[0] = 3;
						knightP[1] = 4;
						break;
				}

				int pl = 0;
				foreach (int i in knightP)
                {
                    int v = i;
					if (b[v] == 0)
					{
						SetPiece(Piece.KNIGHT, Player.WHITE, knightP[pl], 0);
						SetPiece(Piece.KNIGHT, Player.BLACK, knightP[pl], 7);
                        b[v] = 1;
					} else if ((b[v+1]) == 0)
                    {
						SetPiece(Piece.KNIGHT, Player.WHITE, knightP[pl]+1, 0);
						SetPiece(Piece.KNIGHT, Player.BLACK, knightP[pl]+1, 7);
						b[v+1] = 1;
					} else if ((b[v + 2]) == 0)
					{
						SetPiece(Piece.KNIGHT, Player.WHITE, knightP[pl] + 2, 0);
						SetPiece(Piece.KNIGHT, Player.BLACK, knightP[pl] + 2, 7);
						b[v+2] = 1;
					}
                    pl++;
				}

                int R1 = Array.IndexOf(b,0);
				SetPiece(Piece.ROOK, Player.WHITE, R1, 0);
				SetPiece(Piece.ROOK, Player.BLACK, R1, 7);
                b[R1] = 1;

                int kingP = Array.IndexOf(b, 0);
				SetPiece(Piece.KING, Player.WHITE, kingP, 0);
				SetPiece(Piece.KING, Player.BLACK, kingP, 7);
				Kings[Player.WHITE] = new position_t(kingP, 0);
				Kings[Player.BLACK] = new position_t(kingP, 7);
                b[kingP] = 1;

				int R2 = Array.IndexOf(b, 0);
                SetPiece(Piece.ROOK, Player.WHITE, R2, 0);
				SetPiece(Piece.ROOK, Player.BLACK, R2, 7);
				b[R2] = 1;

			}
        }

        public void SetPiece(Piece piece, Player player, int letter, int number)
        {
            // set grid values
            Grid[number][letter].piece = piece;
            Grid[number][letter].player = player;

            // add piece to list
            Pieces[player].Add(new position_t(letter, number));

            // update king position
            if (piece == Piece.KING)
            {
                Kings[player] = new position_t(letter, number);
            }
        }
    }
}
