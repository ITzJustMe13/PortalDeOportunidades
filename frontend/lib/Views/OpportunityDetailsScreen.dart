import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:frontend/Components/ReviewCard.dart';
import 'package:frontend/Models/Review.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:latlong2/latlong.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityImages.dart';
import 'package:frontend/Components/OpportunityLocationMap.dart';
import 'package:frontend/Components/OpportunityDetails.dart';
import 'package:frontend/Components/OpportunityAdditionalInfo.dart';
import 'package:frontend/Components/ReservationButton.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:intl/intl.dart';

class OpportunityDetailsScreen extends StatefulWidget {
  final Opportunity opportunity;

  const OpportunityDetailsScreen({super.key, required this.opportunity});

  @override
  _OpportunityManagerScreenState createState() => _OpportunityManagerScreenState();
}

class _OpportunityManagerScreenState extends State<OpportunityDetailsScreen>{
  final ScrollController verticalScrollController = ScrollController();
  final ScrollController horizontalScrollController = ScrollController();
  late Future<User?> _ownerFuture;

  @override
  void initState() {
    super.initState();
    _ownerFuture = Provider.of<UserApiHandler>(context, listen: false)
        .getUserByID(widget.opportunity.userId);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: FutureBuilder<User?>(
        future: _ownerFuture,
        builder: (context, snapshot) {
          /*
          if (snapshot.connectionState == ConnectionState.waiting) {
            return Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          } else if (!snapshot.hasData || snapshot.data == null) {
            return Center(child: Text('Owner not found'));
          }
          
          final owner = null;//snapshot.data!;
          */
          final opportunity = widget.opportunity;
          final DateTime dateTime = opportunity.date;
          final String formattedTime = DateFormat('HH:mm').format(dateTime);

          //FALTA OWNER DEVIDO A FALTA DE TOKEN PRA API HANDLER
          return LayoutBuilder(
            builder: (context, constraints) {
              if (constraints.maxWidth < 600) {
                return _buildMobileLayout(opportunity, formattedTime);
              } else if (constraints.maxWidth < 1200) {
                return _buildTabletLayout(opportunity, formattedTime);
              } else {
                return _buildDesktopLayout(opportunity, formattedTime);
              }
            },
          );
        },
      ),
    );
  }


  // Mobile layout (Vertical scroll)
  Widget _buildMobileLayout(Opportunity opportunity, String time) {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        children: [
          OpportunityImages(opportunity: opportunity),
          SizedBox(height: 20),
          OpportunityDetails(
            name: opportunity.name,
            description: opportunity.description,),
          SizedBox(height: 20),
          ReservationButton(
            availableVacancies: 2,
            onPressed: () {
              print('Reservation button pressed!');
            },
          ),
          SizedBox(height: 20),
          OpportunityAdditionalInfo(
            price: opportunity.price,
            location: opportunity.location,
            address: opportunity.address,
            vacancies: opportunity.vacancies,
            firstName: "teste",
            lastName: "teste",
            time: time,
            date: opportunity.date,
          ),
          SizedBox(height: 20),
          OpportunityLocationMap(address: opportunity.address,),
          /*
          //REVIEWS
          SizedBox(height: 20),
          Text(
          'Opiniões',
          style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        ),
        SizedBox(height: 10),
        ..._reviews.map((review) => ReviewCard(
            rating: review.rating,
            description: review.description ?? '',
            )).toList(),
            */
        ],
      ),
    );
  }

  // Tablet layout (Vertical scroll with Scrollbar)
    Widget _buildTabletLayout(Opportunity opportunity, String time) {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left Column (Image and Details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityImages(opportunity: opportunity),
                  OpportunityDetails(
                    name: opportunity.name,
                    description: opportunity.description,
                  ),
                  ReservationButton(
                    availableVacancies: opportunity.vacancies,
                    onPressed: () {
                      print('Reservation button pressed!');
                    },
                  ),
                ],
              ),
            ),
            SizedBox(width: 20), // Add space between the two columns
            
            // Right Column (Additional Info and Map)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: opportunity.price,
                    location: opportunity.location,
                    address: opportunity.address,
                    vacancies: opportunity.vacancies,
                    firstName: "teste",
                    lastName: "teste",
                    time: time,
                    date: opportunity.date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(address: opportunity.address,),
                  /*
                  //REVIEWS
                  SizedBox(height: 20),
                  Text(
                  'Opiniões',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  ..._reviews.map((review) => ReviewCard(
                      rating: review.rating,
                      description: review.description ?? '',
                      )).toList(),
                      */
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

 // Desktop layout (Both vertical and horizontal scroll with Scrollbar)
  Widget _buildDesktopLayout(Opportunity opportunity, String time) {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        scrollDirection: Axis.vertical,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left column (images and details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  SizedBox(height: 20),
                  OpportunityImages(opportunity: opportunity),
                  OpportunityDetails(
                    name: opportunity.name,
                    description: opportunity.description,),
                  SizedBox(height: 20),
                  ReservationButton(
                    availableVacancies: 2,
                    onPressed: () {
                      print('Reservation button pressed!');
                    },
                  ),
                ],
              ),
            ),
            SizedBox(width: 20),
            // Right column (additional info and map)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: opportunity.price,
                    location: opportunity.location,
                    address: opportunity.address,
                    vacancies: opportunity.vacancies,
                    firstName: "teste",
                    lastName: "teste",
                    time: time,
                    date: opportunity.date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(address: opportunity.address,),
                  /*
                  //REVIEWS
                  SizedBox(height: 20),
                  Text(
                  'Opiniões',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  ..._reviews.map((review) => ReviewCard(
                      rating: review.rating,
                      description: review.description ?? '',
                      )).toList(),
                      */
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

