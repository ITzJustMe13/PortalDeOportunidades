import 'dart:convert';

import 'package:dynamic_height_grid_view/dynamic_height_grid_view.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Components/ConfirmationDialog.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/State/HistoryReservationState.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:provider/provider.dart';

class HistoryReservationScreen extends StatelessWidget {
  const HistoryReservationScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<HistoryReservationState>(
      builder: (context, historyReservationState, child) => Scaffold(
        appBar: CustomAppBar(),
        endDrawer: CustomDrawer(),
        body: LayoutBuilder(
          builder: (context, constraints) {
            if (constraints.maxWidth < 800) {
              // Layout para telas pequenas (smartphones)
              return _buildMobileLayout(historyReservationState);
            } else if (constraints.maxWidth < 1100) {
              // Layout para telas médias (tablets)
              return _buildTabletLayout(historyReservationState);
            } else {
              // Layout para telas grandes (desktops)
              return _buildDesktopLayout(historyReservationState);
            }
          },
        ),
      ),
    );
  }

  Widget _buildMobileLayout(HistoryReservationState historyReservationState) {
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
          if (historyReservationState.isLoading)
            Center(
              child: CircularProgressIndicator(),
            )
          else if (historyReservationState.reservationList.isEmpty)
            Center(
              child: Text(
                'Nenhuma reserva encontrada',
                style: TextStyle(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                ),
              ),
            )
          else
            ListView.builder(
              shrinkWrap: true,
              physics: NeverScrollableScrollPhysics(),
              itemCount: historyReservationState.reservationList.length,
              itemBuilder: (context, index) {
                return Padding(
                  padding: const EdgeInsets.symmetric(
                      horizontal: 75.0, vertical: 10.0),
                  child: _buildReservationCard(
                      historyReservationState,
                      historyReservationState.reservationList[index].keys.first,
                      historyReservationState
                          .reservationList[index].values.first,
                      context),
                );
              },
            ),
        ],
      ),
    );
  }

  Widget _buildTabletLayout(HistoryReservationState historyReservationState) {
    return SingleChildScrollView(
      scrollDirection: Axis.vertical,
      child: Center(
        child: Padding(
          padding: EdgeInsets.all(24.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.center,
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
              if (historyReservationState.isLoading)
                Center(
                  child: CircularProgressIndicator(),
                )
              else if (historyReservationState.reservationList.isEmpty)
                Center(
                  child: Text(
                    'Nenhuma reserva encontrada',
                    style: TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                )
              else
                LayoutBuilder(
                  builder: (context, constraints) {
                    int itemsPerRow = 3;
                    double totalSpacing = (itemsPerRow - 1) * 16;
                    double cardWidth =
                        (constraints.maxWidth - 2 * 16 - totalSpacing) /
                            itemsPerRow;
                    return DynamicHeightGridView(
                      crossAxisCount: itemsPerRow,
                      shrinkWrap: true,
                      physics: NeverScrollableScrollPhysics(),
                      crossAxisSpacing: 16.0,
                      mainAxisSpacing: 20.0,
                      itemCount: historyReservationState.reservationList.length,
                      builder: (context, index) {
                        return SizedBox(
                          width: cardWidth,
                          child: _buildReservationCard(
                              historyReservationState,
                              historyReservationState
                                  .reservationList[index].keys.first,
                              historyReservationState
                                  .reservationList[index].values.first,
                              context),
                        );
                      },
                    );
                  },
                ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildDesktopLayout(HistoryReservationState historyReservationState) {
    return SingleChildScrollView(
      scrollDirection: Axis.vertical,
      child: Padding(
        padding: const EdgeInsets.all(50.0),
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
            if (historyReservationState.isLoading)
              Center(
                child: CircularProgressIndicator(),
              )
            else if (historyReservationState.reservationList.isEmpty)
              Center(
                child: Text(
                  'Nenhuma reserva encontrada',
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              )
            else
              LayoutBuilder(
                builder: (context, constraints) {
                  int itemsPerRow = 3;

                  double totalSpacing = (itemsPerRow - 1) * 16;
                  double cardWidth =
                      (constraints.maxWidth - 2 * 50 - totalSpacing) /
                          itemsPerRow;

                  return DynamicHeightGridView(
                    crossAxisCount: itemsPerRow,
                    shrinkWrap: true,
                    physics: NeverScrollableScrollPhysics(),
                    crossAxisSpacing: 16.0,
                    mainAxisSpacing: 20.0,
                    itemCount: historyReservationState.reservationList.length,
                    builder: (context, index) {
                      return SizedBox(
                        width: cardWidth,
                        child: _buildReservationCard(
                            historyReservationState,
                            historyReservationState
                                .reservationList[index].keys.first,
                            historyReservationState
                                .reservationList[index].values.first,
                            context),
                      );
                    },
                  );
                },
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildReservationCard(HistoryReservationState state,
      Reservation reservation, Opportunity opportunity, context) {
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
            Stack(
              children: [
                _buildCardImage(opportunity.opportunityImgs.isNotEmpty
                    ? opportunity.opportunityImgs[0].imageBase64
                    : null), // Imagem sem padding.
                Positioned(
                  top: 8,
                  left: 8,
                  child: _buildCategoryTag(
                    opportunity.category.toString().split('.').last,
                  ), // TaTag sobre a imagem.
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
                          opportunity.name,
                        ),
                      ),
                      _buildStatusTag(reservation.isActive),
                    ],
                  ),
                  SizedBox(height: 4),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      _buildLocation(
                          opportunity.location.toString().split('.').last),
                      if (reservation.isActive)
                        _buildDetailsButton(context, opportunity),
                    ],
                  ),
                  SizedBox(height: 8),
                  _buildReservationDates(
                      reservation.reservationDate.toString().split(' ')[0],
                      reservation.checkInDate.toString().split(' ')[0]),
                  SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      _buildNumPeople(reservation.numOfPeople),
                      _buildPrice(reservation.fixedPrice),
                    ],
                  ),
                  SizedBox(height: 8),
                  _buildCancelButton(state, reservation, context),
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

  Widget _buildCardImage(String? image) {
    if (image == null) {
      return Container(
        height: 250,
        width: double.infinity,
        decoration: BoxDecoration(
          color: Colors.grey[300],
          borderRadius: BorderRadius.circular(8),
        ),
        child: Icon(
          Icons.broken_image,
          size: 80,
          color: Colors.grey[600],
        ),
      );
    } else {
      final decodedBytes = base64Decode(image);

      return ClipRRect(
        borderRadius: BorderRadius.vertical(top: Radius.circular(8)),
        child: Image.memory(
          decodedBytes,
          height: 250,
          width: double.infinity,
          fit: BoxFit.cover,
          errorBuilder: (context, error, stackTrace) {
            return Center(
              child: Icon(
                Icons.broken_image,
                color: Colors.grey,
                size: 50,
              ),
            );
          },
        ),
      );
    }
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

  Widget _buildDetailsButton(BuildContext context, Opportunity opportunity) {
    return DynamicActionButton(
      text: 'Detalhes',
      color: Color(0xFF50C878),
      icon: Icons.details,
      onPressed: () {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => OpportunityDetailsScreen(
              opportunity: opportunity,
              isReservation: true,
            ),
          ),
        );
      },
    );
  }

  Widget _buildCancelButton(HistoryReservationState historyReservationState,
      Reservation reservation, BuildContext context) {
    return Column(
      children: [
        if (historyReservationState.isCancelling)
          Center(
            child: CircularProgressIndicator(),
          )
        else
          Align(
            alignment: Alignment.centerRight, // Alinha o botão à direita.
            child: Tooltip(
              message:
                  'Para obter o reembolso parcial da reserva contacte o suporte.',
              child: DynamicActionButton(
                text: 'Cancelar',
                icon: Icons.cancel,
                color: Colors.red,
                onPressed: () async {
                  bool confirmCancellation = await showDialog<bool>(
                        context: context,
                        builder: (BuildContext context) {
                          return ConfirmationDialog(
                            action: 'Cancelar Reserva',
                            message:
                                'Tem certeza de que deseja cancelar esta reserva?\n\n*Para obter o reembolso parcial da reserva contacte o suporte.',
                            onConfirm: () async {
                              final bool success = await historyReservationState
                                  .cancelReservation(reservation);

                              return success;
                            },
                          );
                        },
                      ) ??
                      false;

                  if (confirmCancellation) {
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('Reserva cancelada com sucesso!'),
                        backgroundColor: Colors.green,
                      ),
                    );
                  } else {
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('Falha ao cancelar a reserva.'),
                        backgroundColor: Colors.red,
                      ),
                    );
                  }
                },
              ),
            ),
          ),
        if (historyReservationState.error.isNotEmpty)
          Text(historyReservationState.error,
              style: TextStyle(color: Colors.red)),
      ],
    );
  }
}
