import 'package:dynamic_height_grid_view/dynamic_height_grid_view.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/dynamic_details_button.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';

class FavoritesPage extends StatefulWidget {
  const FavoritesPage({super.key});

  @override
  State<FavoritesPage> createState() => _FavoritesPageState();
}

class _FavoritesPageState extends State<FavoritesPage> {
  Opportunity opportunity = Opportunity(
      name: "Oportunidade9",
      price: 10.2,
      vacancies: 1,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: "description",
      location: Location.ACORES,
      address: "address",
      userId: 1,
      reviewScore: 42,
      date: DateTime.now(),
      isImpulsed: false,
      opportunityImgs: []);

  @override
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
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
              'Os seus Favoritos:',
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
                child: _buildOpportunityCard(opportunity),
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
      child: Center(
        child: Padding(
          padding: EdgeInsets.all(24.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              Padding(
                padding: const EdgeInsets.all(16.0),
                child: Text(
                  'Os seus Favoritos:',
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              LayoutBuilder(
                builder: (context, constraints) {
                  int itemsPerRow = 3;
                  // Calcular a largura dos cards considerando padding e spacing
                  double totalSpacing =
                      (itemsPerRow - 1) * 16; // Espaço entre os cards
                  double cardWidth =
                      (constraints.maxWidth - 2 * 16 - totalSpacing) /
                          itemsPerRow; // Largura dos cards
                  return DynamicHeightGridView(
                    crossAxisCount: itemsPerRow,
                    shrinkWrap: true,
                    physics: NeverScrollableScrollPhysics(),
                    crossAxisSpacing: 16.0,
                    mainAxisSpacing: 20.0,
                    itemCount: 6,
                    builder: (context, index) {
                      return SizedBox(
                        width:
                            cardWidth, // Largura ajustada para 3 cards por linha
                        child: _buildOpportunityCard(opportunity),
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

  Widget _buildDesktopLayout() {
    return SingleChildScrollView(
      scrollDirection: Axis.vertical, // Definindo scroll vertical
      child: Padding(
        padding:
            const EdgeInsets.symmetric(horizontal: 96.0, vertical: 24.0), // Ajustando o padding
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: const EdgeInsets.all(50.0),
              child: Text(
                'Os seus Favoritos:',
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

                return DynamicHeightGridView(
                  crossAxisCount: itemsPerRow,
                  shrinkWrap: true,
                  physics: NeverScrollableScrollPhysics(),
                  crossAxisSpacing: 16.0,
                  mainAxisSpacing: 20.0,
                  itemCount: 6,
                  builder: (context, index) {
                    return SizedBox(
                      width:
                          cardWidth, // Largura ajustada para 3 cards por linha
                      child: _buildOpportunityCard(opportunity),
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

  Widget _buildOpportunityCard(Opportunity opportunity) {
    return ConstrainedBox(
      constraints: BoxConstraints(),
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
                    'https://picsum.photos/200'), // Imagem sem padding.
                Positioned(
                  top: 8,
                  left: 8,
                  child: _buildCategoryTag(
                    opportunity.category.toString().split('.').last,
                  ), // Tag sobre a imagem.
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
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Flexible(
                        fit: FlexFit.tight,
                        flex: 2,
                        child: _buildTitle(
                          'Venha aprender a cozinhar o Pato à Transmontana',
                        ),
                      ),
                      SizedBox(width: 8),
                      Flexible(
                          fit: FlexFit.loose, child: DynamicDetailsButton()),
                    ],
                  ),
                  SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      _buildLocation(
                        opportunity.location.toString().split('.').last,
                      ),
                      _buildRemoveButton(),
                    ],
                  ),
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

  Widget _buildRemoveButton() {
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
          'Remover',
          style: TextStyle(color: Colors.white),
        ),
      ),
    );
  }
}
