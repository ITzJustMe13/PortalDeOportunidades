import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/Components/ConfirmationDialog.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Views/FavoritesPage.dart';
import 'package:frontend/Services/user_api_handler.dart';

class FavoriteCard extends StatelessWidget {
  final Opportunity opportunity;
  final dynamic user; // Replace with your User type if available

  const FavoriteCard({
    Key? key,
    required this.opportunity,
    required this.user,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 4,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildImageSection(opportunity),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20.0, vertical: 10.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildTitle(opportunity.name),
                _buildLocation(opportunity.location.toString().split('.').last),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    DynamicActionButton(
                      text: 'Detalhes',
                      color: const Color(0xFF50C878),
                      icon: Icons.details,
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) =>
                                OpportunityDetailsScreen(opportunity: opportunity),
                          ),
                        );
                      },
                    ),
                    DynamicActionButton(
                      text: "Remover",
                      icon: Icons.remove,
                      color: Colors.red,
                      onPressed: () {
                        if (user != null) {
                          removeFavorite(
                            context,
                            opportunity.opportunityId,
                            user.userId,
                          );
                        }
                      },
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildImageSection(Opportunity opportunity) {
    return Stack(
      children: [
        opportunity.opportunityImgs.isNotEmpty
            ? Image.memory(
                base64Decode(opportunity.opportunityImgs.first.imageBase64),
                fit: BoxFit.cover,
                height: 150,
                width: double.infinity,
                errorBuilder: (context, error, stackTrace) {
                  return Container(
                    height: 150,
                    width: double.infinity,
                    color: Colors.grey,
                    child: const Center(
                      child: Icon(
                        Icons.broken_image,
                        color: Colors.white,
                      ),
                    ),
                  );
                },
              )
            : Container(
                height: 150,
                width: double.infinity,
                color: Colors.grey,
                child: const Center(
                  child: Icon(
                    Icons.image_not_supported,
                    color: Colors.white,
                  ),
                ),
              ),
        Positioned(
          top: 0,
          left: 0,
          child: Container(
            padding: const EdgeInsets.all(4.0),
            color: Colors.black54,
            child: Text(
              opportunity.category.toString().split('.').last,
              style: const TextStyle(
                color: Colors.white,
                fontSize: 14,
              ),
            ),
          ),
        ),
        Positioned(
          bottom: 0,
          left: 0,
          child: Container(
            padding: const EdgeInsets.all(4.0),
            color: Colors.black54,
            child: Column(
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                Text(
                  "${opportunity.price.toString()}€ /Pessoa",
                  style: const TextStyle(fontSize: 12, color: Colors.white),
                ),
                Text(
                  opportunity.location.toString().split('.').last,
                  style: const TextStyle(fontSize: 14, color: Colors.white),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildTitle(String title) {
    return Text(
      title,
      style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
    );
  }

  Widget _buildLocation(String location) {
    return Text(
      location,
      style: const TextStyle(fontSize: 24, color: Colors.grey),
    );
  }

  /// Documentation for removeFavortites
  /// @param: BuildContext context
  /// @param: int oppId Opportunity id
  /// @param: int userId User id
  /// This Function removes an Opportunity from a user's Favorites
  static Future<void> removeFavorite(BuildContext context, int oppId, int userId) async {
    bool confirmDelete = await showDialog<bool>(
      context: context,
      builder: (BuildContext context) {
        return ConfirmationDialog(
          action: 'remover',
          message: 'Tem certeza de que deseja remover este favorito?',
          onConfirm: () async {
            final bool success = await Provider.of<UserApiHandler>(context, listen: false)
                .deleteFavoriteById(userId, oppId);
            return success;
          },
        );
      },
    ) ?? false;

    if (!context.mounted) return; // Check if the context is still valid

    if (confirmDelete) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Favorito removido com sucesso!'),
          backgroundColor: Colors.green,
        ),
      );

      if (context.mounted) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => FavoritesPage()),
        );
      }
    } else {
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Falha ao remover Favorito'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }
}
