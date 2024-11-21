import 'package:flutter/material.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Reservation.dart';

class HistoryReservationScreen extends StatefulWidget {
  @override
  _HistoryReservationScreenState createState() =>
      _HistoryReservationScreenState();
}

class _HistoryReservationScreenState extends State<HistoryReservationScreen> {
  Reservation reservation = Reservation(
      opportunityId: 1,
      userId: 1,
      reservationDate: DateTime.now(),
      checkInDate: DateTime.now(),
      numOfPeople: 3,
      isActive: true,
      fixedPrice: 12.20);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Histórico de Reservas'),
      ),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 800) {
            // Layout para telas pequenas (smartphones)
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1100) {
            // Layout para telas médias (tablets)
            return _buildTabletLayout();
          } else {
            // Layout para telas grandes (desktops)
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Text(
              'Os seus Histórico de Reservas:',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          ListView.builder(
            shrinkWrap: true,
            physics: NeverScrollableScrollPhysics(),
            itemCount: 3, // Pode ser uma lista dinâmica no futuro.
            itemBuilder: (context, index) {
              return Padding(
                padding: const EdgeInsets.symmetric(
                    horizontal: 75.0, vertical: 10.0),
                child: _buildReservationCard(reservation),
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return SingleChildScrollView(
      scrollDirection: Axis.vertical, // Para garantir o scroll vertical
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Text(
              'Os seus Histórico de Reservas:',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          LayoutBuilder(
            builder: (context, constraints) {
              // Número de itens por linha para tablet
              int itemsPerRow = 2;
              // Calcular a largura dos cards considerando padding e spacing
              double totalSpacing =
                  (itemsPerRow - 1) * 16; // Espaço entre os cards
              double cardWidth =
                  (constraints.maxWidth - 2 * 16 - totalSpacing) /
                      itemsPerRow; // Largura dos cards

              return Padding(
                padding: const EdgeInsets.symmetric(
                    horizontal:
                        16.0), // Mantém o padding correto nas extremidades
                child: Wrap(
                  spacing: 16, // Espaço horizontal entre os cards
                  runSpacing: 20, // Espaço vertical entre as linhas
                  children: List.generate(6, (index) {
                    return SizedBox(
                      width:
                          cardWidth, // Largura ajustada para 2 cards por linha
                      child: _buildReservationCard(reservation),
                    );
                  }),
                ),
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _buildDesktopLayout() {
    return SingleChildScrollView(
      scrollDirection: Axis.vertical, // Definindo scroll vertical
      child: Padding(
        padding:
            const EdgeInsets.symmetric(horizontal: 50.0), // Ajustando o padding
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: const EdgeInsets.all(50.0),
              child: Text(
                'O seu Histórico de Reservas:',
                style: TextStyle(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            LayoutBuilder(
              builder: (context, constraints) {
                // Número de itens por linha
                int itemsPerRow = 3;

                // Calcular a largura dos cartões, levando em consideração o espaçamento e o padding
                double totalSpacing =
                    (itemsPerRow - 1) * 16; // Espaço horizontal entre os cards
                double cardWidth =
                    (constraints.maxWidth - 2 * 50 - totalSpacing) /
                        itemsPerRow;

                return Wrap(
                  spacing: 16, // Espaço horizontal entre os cards
                  runSpacing: 20, // Espaço vertical entre as linhas
                  children: List.generate(6, (index) {
                    return SizedBox(
                      width:
                          cardWidth, // Largura ajustada para 3 cards por linha
                      child: _buildReservationCard(reservation),
                    );
                  }),
                );
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildReservationCard(Reservation reservation) {
    return ConstrainedBox(
      constraints: BoxConstraints(
        maxHeight: 500, // Limita a largura do card
      ),
      child: Card(
        elevation: 4,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Imagem e tag empilhadas (sem padding).
            Stack(
              children: [
                _buildCardImage(
                    'https://via.placeholder.com/300'), // Imagem sem padding.
                Positioned(
                  top: 8,
                  left: 8,
                  child: _buildCategoryTag(OppCategory
                      .DESPORTOS_ATIVIDADES_AO_AR_LIVRE
                      .toString()), // Tag sobre a imagem.
                ),
              ],
            ),
            Padding(
              padding: const EdgeInsets.symmetric(
                  horizontal: 20.0,
                  vertical: 10.0), // Adiciona padding aos lados
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      ConstrainedBox(
                        constraints: BoxConstraints(
                            maxWidth: 250), // Limita a largura a 250px
                        child: _buildTitle(
                          'Venha aprender a cozinhar o Pato à Transmontana',
                        ),
                      ),
                      _buildStatusTag(reservation.isActive),
                    ],
                  ),
                  SizedBox(height: 4),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      _buildLocation('Vila Real'),
                      _buildDetailsButton(),
                    ],
                  ),
                  SizedBox(height: 8),
                  _buildReservationDates('02/02/2025', '04/02/2025'),
                  SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      _buildNumPeople(2),
                      _buildPrice(23.44),
                    ],
                  ),
                  SizedBox(height: 8),
                  // Botões.
                  _buildCancelButton(),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildCategoryTag(String category) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.green.shade100,
        borderRadius: BorderRadius.circular(8),
      ),
      padding: const EdgeInsets.symmetric(vertical: 4, horizontal: 8),
      child: Text(
        category,
        style: TextStyle(
          color: Colors.green,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }

  Widget _buildStatusTag(bool isActive) {
    return Container(
      decoration: BoxDecoration(
        color: isActive ? Colors.green.shade100 : Colors.red.shade100,
        borderRadius: BorderRadius.circular(8),
      ),
      padding: const EdgeInsets.symmetric(vertical: 4, horizontal: 8),
      child: Text(
        isActive ? 'ATIVA' : 'INATIVA',
        style: TextStyle(
          color: isActive ? Colors.green : Colors.red,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }

  Widget _buildCardImage(String imageUrl) {
    return ClipRRect(
      borderRadius: BorderRadius.vertical(top: Radius.circular(8)),
      child: Image.network(
        imageUrl,
        height: 250,
        width: double.infinity,
        fit: BoxFit.cover,
        loadingBuilder: (context, child, progress) {
          if (progress == null) return child; // Imagem carregada.
          return Center(
            child: CircularProgressIndicator(), // Indicador de carregamento.
          );
        },
        errorBuilder: (context, error, stackTrace) {
          return Center(
            child: Icon(
              Icons.broken_image,
              color: Colors.grey,
              size: 50,
            ), // Ícone de erro para imagem inválida.
          );
        },
      ),
    );
  }

  Widget _buildTitle(String title) {
    return Text(
      title,
      style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
    );
  }

  Widget _buildLocation(String location) {
    return Text(
      location,
      style: TextStyle(fontSize: 24, color: Colors.grey),
    );
  }

  Widget _buildReservationDates(String reservedDate, String checkInDate) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Column(
          crossAxisAlignment:
              CrossAxisAlignment.center, // Centraliza os textos na coluna.
          children: [
            Text(
              'Reservada em:',
              style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold), // Estilização opcional.
            ),
            SizedBox(height: 4), // Espaço entre o título e a data.
            Text(reservedDate),
          ],
        ),
        Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Text(
              'Realizada em:',
              style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold), // Estilização opcional.
            ),
            SizedBox(height: 4), // Espaço entre o título e a data.
            Text(checkInDate),
          ],
        ),
      ],
    );
  }

  Widget _buildNumPeople(int numPeople) {
    return Text('$numPeople Pessoas', style: TextStyle(fontSize: 16));
  }

  Widget _buildPrice(double price) {
    return Text(
      'Total: ${price.toStringAsFixed(2)}€',
      style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
    );
  }

  Widget _buildDetailsButton() {
    return ElevatedButton(
      onPressed: () {
        // Lógica para o botão "Detalhes"
      },
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.green,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        padding: const EdgeInsets.symmetric(vertical: 4, horizontal: 12),
      ),
      child: Text(
        'Detalhes',
        style: TextStyle(color: Colors.white),
      ),
    );
  }

  Widget _buildCancelButton() {
    return Align(
      alignment: Alignment.centerRight, // Alinha o botão à direita.
      child: ElevatedButton(
        onPressed: () {
          // Lógica para o botão "Cancelar"
        },
        style: ElevatedButton.styleFrom(
          backgroundColor: Colors.red,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(8),
          ),
          padding: const EdgeInsets.symmetric(vertical: 4, horizontal: 12),
        ),
        child: Text(
          'Cancelar',
          style: TextStyle(color: Colors.white),
        ),
      ),
    );
  }
}
