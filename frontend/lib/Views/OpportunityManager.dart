import 'package:dynamic_height_grid_view/dynamic_height_grid_view.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityManageCard.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:provider/provider.dart';

class OpportunityManagerScreen extends StatefulWidget {
  const OpportunityManagerScreen({super.key});

  @override
  _OpportunityManagerScreenState createState() =>
      _OpportunityManagerScreenState();
}

class _OpportunityManagerScreenState extends State<OpportunityManagerScreen> {
  late Future<List<Opportunity>?> _opportunitiesFuture;

  User? user;

  @override
  void initState() {
    super.initState();
    _opportunitiesFuture = Future.value(
        []); //this is to give time to load and not give any errors to the page
    _initializeData();
  }

  Future<void> _initializeData() async {
    user = await _getCachedUser();
    if (user != null) {
      _opportunitiesFuture =
          Provider.of<OpportunityApiHandler>(context, listen: false)
              .getAllOpportunitiesByUserId(user!.userId);
      setState(() {}); // Trigger a rebuild after setting opportunities
    } else {
      _opportunitiesFuture = Future.value([]);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: FutureBuilder<List<Opportunity>?>(
        future: _opportunitiesFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(
              child: Text('Erro ao carregar Opportunidades: ${snapshot.error}'),
            );
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(
                child: Text('Não foram encontradas Oportunidades.'));
          } else {
            final opportunities = snapshot.data!;
            return LayoutBuilder(
              builder: (context, constraints) {
                if (constraints.maxWidth < 600) {
                  return _buildMobileLayout(opportunities);
                } else if (constraints.maxWidth < 1200) {
                  return _buildTabletLayout(opportunities);
                } else {
                  return _buildDesktopLayout(opportunities);
                }
              },
            );
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout(List<Opportunity> opportunities) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20.0),
      child: Column(
        children: [
          Text(
            "As suas Oportunidades",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          ...opportunities.map((opportunity) => Padding(
                padding: const EdgeInsets.only(bottom: 20.0),
                child: OpportunityManageCard(opportunity: opportunity),
              )),
        ],
      ),
    );
  }

  Widget _buildTabletLayout(List<Opportunity> opportunities) {
    return SingleChildScrollView(
      padding: const EdgeInsets.symmetric(horizontal: 60.0, vertical: 20.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "As suas Oportunidades",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          LayoutBuilder(
            builder: (context, constraints) {
              // Define the minimum width for each card
              const double minCardWidth = 350;

              // Calculate the number of items that can fit based on the available width
              int itemsPerRow =
                  (constraints.maxWidth / minCardWidth).floor().clamp(1, 5);

              return ConstrainedBox(
                constraints: BoxConstraints(
                  maxHeight: opportunities.length /
                      itemsPerRow *
                      1500, // Approximate height
                ),
                child: DynamicHeightGridView(
                  crossAxisCount: itemsPerRow,
                  crossAxisSpacing: 10,
                  mainAxisSpacing: 20,
                  physics: NeverScrollableScrollPhysics(),
                  itemCount: opportunities.length,
                  builder: (context, index) {
                    return OpportunityManageCard(
                        opportunity: opportunities[index]);
                  },
                ),
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _buildDesktopLayout(List<Opportunity> opportunities) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(60.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "As suas Oportunidades",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          LayoutBuilder(
            builder: (context, constraints) {
              // Define the minimum width for each card
              const double minCardWidth = 400;

              // Calculate the number of items that can fit based on the available width
              int itemsPerRow =
                  (constraints.maxWidth / minCardWidth).floor().clamp(1, 5);

              return ConstrainedBox(
                constraints: BoxConstraints(
                  maxHeight: opportunities.length /
                      itemsPerRow *
                      1500, // Approximate height
                ),
                child: DynamicHeightGridView(
                  crossAxisCount: itemsPerRow,
                  crossAxisSpacing: 20,
                  mainAxisSpacing: 30,
                  physics: NeverScrollableScrollPhysics(),
                  itemCount: opportunities.length,
                  builder: (context, index) {
                    return OpportunityManageCard(
                        opportunity: opportunities[index]);
                  },
                ),
              );
            },
          ),
        ],
      ),
    );
  }

  Future<User?> _getCachedUser() async {
    try {
      // Fetch the user from the API or local storage
      final user = await Provider.of<UserApiHandler>(context, listen: false)
          .getStoredUser();
      return user;
    } catch (e) {
      return null;
    }
  }
}
