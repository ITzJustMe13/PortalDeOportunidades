import 'package:dynamic_height_grid_view/dynamic_height_grid_view.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Models/Favorite.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/user_services.dart';

import 'package:frontend/Services/user_api_handler.dart';
import 'package:provider/provider.dart';

import 'package:frontend/Components/FavoriteCard.dart';

/// Documentation for FavoritesPage
/// this page shows the user his favorites._
class FavoritesPage extends StatefulWidget {
  const FavoritesPage({super.key});

  @override
  State<FavoritesPage> createState() => _FavoritesPageState();
}

/// Documentation for FavoritesPageState
/// this is the state of the page for it to be able to 
/// update without exterior libraries
class _FavoritesPageState extends State<FavoritesPage> {
  late Future<List<Favorite>?> _favoritesFuture;
  late Future<List<Opportunity>?> _opportunityFuture = Future.value([]);
  late User? _user;

  @override
  void initState() {
    super.initState();
    _loadData(context);
  }

/// Documentation for build
/// Builds the screen and categorizes for
/// Mobile, Table and Desktop
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 800) {
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1100) {
            return _buildTabletLayout();
          } else {
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

/// Documentation for _buildMobileLayout
/// Mobile layout of the screen
  Widget _buildMobileLayout() {
    return FutureBuilder<List<Opportunity>?>(
      future: _opportunityFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return Center(child: CircularProgressIndicator());
        } else if (snapshot.hasError) {
          return Center(child: Text('Error: ${snapshot.error}'));
        } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
          return Center(child: Text('Não foram encontrados favoritos.'));
        } else {
          List<Opportunity> opportunities = snapshot.data!;
          return ListView.builder(
            itemCount: opportunities.length,
            itemBuilder: (context, index) {
              return Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    SizedBox(height: 20),
                    Text(
                      "Os seus Favoritos:",
                      style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
                    ),
                    FavoriteCard(opportunity:opportunities[index], user:_user),
                  ],
                ),
              );
            },
          );
        }
      },
    );
  }

/// Documentation for _buildTableLayout
/// Tablet layout of the screen
  Widget _buildTabletLayout() {
    return FutureBuilder<List<Opportunity>?>(
      future: _opportunityFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return Center(child: CircularProgressIndicator());
        } else if (snapshot.hasError) {
          return Center(child: Text('Error: ${snapshot.error}'));
        } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
          return Center(child: Text('Não foram encontrados favoritos.'));
        } else {
          List<Opportunity> opportunities = snapshot.data!;
          return Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(height: 20),
                Text(
                  "Os seus Favoritos:",
                  style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
                ),
                SizedBox(height: 20),
                DynamicHeightGridView(
                  crossAxisCount: 3,
                  shrinkWrap: true,
                  physics: NeverScrollableScrollPhysics(),
                  crossAxisSpacing: 16.0,
                  mainAxisSpacing: 20.0,
                  itemCount: opportunities.length,
                  builder: (context, index) {
                    return FavoriteCard(opportunity:opportunities[index], user:_user);
                  },
                ),
              ],
            ),
          );
        }
      },
    );
  }

/// Documentation for _buildDesktopLayout
/// Desktop layout of the screen
  Widget _buildDesktopLayout() {
    return FutureBuilder<List<Opportunity>?>(
      future: _opportunityFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return Center(child: CircularProgressIndicator());
        } else if (snapshot.hasError) {
          return Center(child: Text('Error: ${snapshot.error}'));
        } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
          return Center(child: Text('Não foram encontrados favoritos.'));
        } else {
          List<Opportunity> opportunities = snapshot.data!;
          return Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(height: 20),
                Text(
                  "Os seus Favoritos:",
                  style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
                ),
                SizedBox(height: 20),
                DynamicHeightGridView(
                  crossAxisCount: 3,
                  shrinkWrap: true,
                  physics: NeverScrollableScrollPhysics(),
                  crossAxisSpacing: 16.0,
                  mainAxisSpacing: 20.0,
                  itemCount: opportunities.length,
                  builder: (context, index) {
                    return FavoriteCard(opportunity:opportunities[index], user:_user);
                  },
                ),
              ],
            ),
          );
        }
      },
    );
  }

/// Documentation for _loadData
/// @param: BuildContext context
/// This function loads the favorites of the user in the initState
  Future<void> _loadData(BuildContext context) async {
    final user = await UserServices.getCachedUser(context); 
    setState(() {
      _user = user;
    });

    if (_user != null) {
      // Fetch the user's favorites if user is found
      _favoritesFuture = Provider.of<UserApiHandler>(context, listen: false).getFavorites(_user!.userId);

      // After fetching favorites, fetch all associated opportunities
      _opportunityFuture = _favoritesFuture.then((favorites) async {
        if (favorites != null && favorites.isNotEmpty) {
          final opportunityIds = favorites.map((favorite) => favorite.opportunityId).toList();

          // Fetch all opportunities in parallel to avoid sequential delays
          final opportunities = await Future.wait(opportunityIds.map((id) async {
            try {
              return await Provider.of<OpportunityApiHandler>(context, listen: false).getOpportunityByID(id);
            } catch (e) {
              return null;
            }
          }));

          // Filter out any null values and return the list
          return opportunities.whereType<Opportunity>().toList();
        }
        return [];
      });
    } else {
      // If user is null, set futures to empty
      setState(() {
        _favoritesFuture = Future.value([]);
        _opportunityFuture = Future.value([]);
      });
    }
  }
}

  

